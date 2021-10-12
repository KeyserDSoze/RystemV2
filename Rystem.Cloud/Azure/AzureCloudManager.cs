using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using Rystem.Text;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;
using System.Net.Http.Json;

namespace Rystem.Cloud.Azure
{
    public sealed class AzureCloudManager : ICloudManagement
    {
        private readonly AzureAadAppRegistration AppRegistration;
        private AzureAccount AzureAccount { get; set; }
        private readonly List<CloudManagementError> Errors = new();
        private readonly HttpClient HttpClient;
        public AzureCloudManager(AzureAadAppRegistration appRegistration, IHttpClientFactory httpClientFactory)
        {
            AppRegistration = appRegistration;
            HttpClient = httpClientFactory.CreateClient("rystem.cloud.azure");
        }
        private bool IsAuthenticated => AzureAccount != null && DateTime.UtcNow >= AzureAccount.StartTime && DateTime.UtcNow <= AzureAccount.EndTime;
        private static readonly JsonSerializerOptions Options = new()
        {
            NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
        };
        private async Task AuthenticateAsync()
        {
            try
            {
                string body = $"grant_type=client_credentials&client_id={AppRegistration.ClientId}&client_secret={AppRegistration.ClientSecret}&resource=https://management.azure.com/";
                AzureAccount = await HttpClient
                    .PostAsync<AzureAccount>($"https://login.microsoftonline.com/{AppRegistration.TenantId}/oauth2/token",
                    new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded"),
                    Options)
                    .NoContext();
            }
            catch (Exception ex)
            {
                Errors.Add(new CloudManagementError(string.Empty, nameof(AuthenticateAsync), CloudManagementErrorType.Authentication, ex));
            }
        }
        private async Task<HttpClient> GetAuthenticatedClientAsync()
        {
            if (!IsAuthenticated)
            {
                await AuthenticateAsync().NoContext();
                HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {AzureAccount.AccessToken}");
            }
            return HttpClient;
        }
        public Task<(Tenant Tenant, List<CloudManagementError> Errors)> GetTenantByMonthAsync(DateTime month, ManagementDeepRequest deepRequest, bool executeRequestInParallel)
            => GetTenantAsync(new DateTime(month.Year, month.Month, 1, 0, 0, 0, 0), new DateTime(month.AddMonths(1).Year, month.AddMonths(1).Month, 1, 23, 59, 59, 999).AddDays(-1), deepRequest, executeRequestInParallel);
        public async Task<(Tenant Tenant, List<CloudManagementError> Errors)> GetTenantAsync(DateTime from, DateTime to, ManagementDeepRequest deepRequest, bool executeRequestInParallel)
            => (new Tenant(string.Empty, await GetSubscriptionsAsync(from, to, deepRequest, executeRequestInParallel).NoContext()), Errors);
        private static readonly Regex RegexToSplitValidMetrics = new("Valid metrics:");
        private static readonly object TrafficLight = new();
        public async Task<IEnumerable<Subscription>> ListSubscriptionsAsync()
            => (await (await GetAuthenticatedClientAsync().NoContext())
                    .GetFromJsonAsync<AzureSubscriptions>($"https://management.azure.com/subscriptions?api-version=2020-01-01", Options)
                        .NoContext())
                    .Subscriptions.Select(x =>
                        new Subscription(x.SubscriptionId, x.TenantId, x.DisplayName, x.State, x.Tags, new())
                );
        private async Task<List<Subscription>> GetSubscriptionsAsync(DateTime startTime, DateTime endTime, ManagementDeepRequest azureDeepRequest, bool executeRequestInParallel)
        {
            List<Subscription> outputSubscriptions = new();
            try
            {
                List<AzureSubscription> subscriptions = (await (await GetAuthenticatedClientAsync().NoContext())
                    .GetFromJsonAsync<AzureSubscriptions>($"https://management.azure.com/subscriptions?api-version=2020-01-01", Options)
                        .NoContext())
                    .Subscriptions;
                List<Task> subscriptionTasks = new();
                foreach (var subscription in subscriptions)
                {
                    if (executeRequestInParallel)
                        subscriptionTasks.Add(SetSubscriptionAsync());
                    else
                        await SetSubscriptionAsync();

                    async Task SetSubscriptionAsync()
                    {
                        var outputSubscription = await GetSubscriptionAsync(subscription, startTime, endTime, azureDeepRequest, executeRequestInParallel).NoContext();
                        if (outputSubscription != default)
                            lock (TrafficLight)
                                outputSubscriptions.Add(outputSubscription);
                    }
                }
                await Task.WhenAll(subscriptionTasks).NoContext();
            }
            catch (Exception ex)
            {
                Errors.Add(new CloudManagementError(string.Empty, string.Empty, CloudManagementErrorType.Subscriptions, ex));
            }
            return outputSubscriptions;
        }
        private async Task<Subscription> GetSubscriptionAsync(AzureSubscription subscription, DateTime startTime, DateTime endTime, ManagementDeepRequest azureDeepRequest, bool executeRequestInParallel)
        {
            try
            {
                var tags = await (await GetAuthenticatedClientAsync().NoContext())
                        .GetFromJsonAsync<AzureTagObject>($"https://management.azure.com{subscription.Id}/tagNames?api-version=2021-04-01")
                        .NoContext();
                if (subscription.Tags == default)
                    subscription.Tags = new();
                if (tags != default && tags.Value.Length > 0)
                {
                    foreach (var tag in tags.Value)
                    {
                        if (!subscription.Tags.ContainsKey(tag.TagName))
                            subscription.Tags.Add(tag.TagName, tag.Values.FirstOrDefault()?.TagValue ?? string.Empty);
                    }
                }
                Subscription outputSubscription = new(subscription.Id, subscription.TenantId, subscription.DisplayName, subscription.State, subscription.Tags, new());
                List<AzureResourceGroup> resourceGroups = await GetResourceGroupsAsync(subscription.SubscriptionId);
                if (resourceGroups == default)
                    return default;
                List<AzureResource> resources = await GetResourcesAsync(subscription.SubscriptionId);
                if (resources == default)
                    return default;
                List<AzureConsumption> consumptions = await GetConsumptionsAsync(subscription.SubscriptionId, startTime, endTime).NoContext();
                List<Cost> costs = azureDeepRequest >= ManagementDeepRequest.Cost ? await GetCostsAsync(subscription.SubscriptionId, startTime, endTime, consumptions) : new();
                foreach (var costByResourceGroupNotMoreAvailable in costs.Where(x => !resourceGroups.Any(t => t.Name.ToLower() == x.ResourceGroup.ToLower())).GroupBy(x => x.ResourceGroup))
                {
                    resourceGroups.Add(new AzureResourceGroup
                    {
                        Id = $"/subscriptions/{subscription.SubscriptionId}/resourceGroups/{costByResourceGroupNotMoreAvailable.Key}",
                        Name = costByResourceGroupNotMoreAvailable.Key,
                    });
                }
                List<Task> resourceGroupTasks = new();
                foreach (AzureResourceGroup resourceGroup in resourceGroups)
                {
                    if (executeRequestInParallel)
                        resourceGroupTasks.Add(SetResourceGroupAsync());
                    else
                        await SetResourceGroupAsync().NoContext();

                    async Task SetResourceGroupAsync()
                    {
                        ResourceGroup outputResourceGroup = new(resourceGroup.Id, resourceGroup.Name, resourceGroup.Location, resourceGroup.Tags, new());
                        List<Resource> resourcesFromResourceGroup = resources
                            .Where(x => x.Id.ToLower().StartsWith($"/subscriptions/{subscription.SubscriptionId}/resourceGroups/{resourceGroup.Name}/".ToLower()))
                            .Select(x => new Resource(x.Id, x.Name, x.Type, x.Kind, x.Location, x.Tags, x.Sku != default ? new Sku(x.Sku.Name, x.Sku.Tier, x.Sku.Capacity, x.Sku.Size, x.Sku.Family) : default, x.ManagedBy, x.Plan != default ? new Plan(x.Plan.Name, x.Plan.PromotionCode, x.Plan.Product, x.Plan.Publisher) : default, new(), new(), new()))
                            .ToList();
                        List<Cost> costByResourceGroup = costs.Where(x => x.ResourceGroup.ToLower() == resourceGroup.Name.ToLower()).ToList();
                        List<Task> resourceTasks = new();
                        foreach (Resource resource in resourcesFromResourceGroup)
                        {
                            if (executeRequestInParallel)
                                resourceTasks.Add(SetResourceValuesAsync());
                            else
                                await SetResourceValuesAsync().NoContext();

                            async Task SetResourceValuesAsync()
                            {
                                if (azureDeepRequest >= ManagementDeepRequest.Monitoring)
                                {
                                    await GetPossibleMetricsAsync(resource).NoContext();
                                    var monitorings = await GetMonitoringsAsync(resource, startTime, endTime, subscription.Id).NoContext();
                                    lock (TrafficLight)
                                        resource.Monitorings.AddRange(monitorings);
                                }
                                lock (TrafficLight)
                                {
                                    resource.Costs.AddRange(costByResourceGroup.Where(x => x.ResourceId.ToLower() == resource.Id.ToLower()));
                                    outputResourceGroup.Resources.Add(resource);
                                }
                            }
                            await Task.WhenAll(resourceTasks).NoContext();
                        };

                        foreach (var costForResourceNotMoreAvailablecostForResourceNotMoreAvailable in costByResourceGroup.Where(x => !resourcesFromResourceGroup.Any(t => t.Id.ToLower() == x.ResourceId.ToLower())).GroupBy(x => x.ResourceId))
                        {
                            var resource = new Resource(
                                costForResourceNotMoreAvailablecostForResourceNotMoreAvailable.Key,
                                costForResourceNotMoreAvailablecostForResourceNotMoreAvailable.Key.Split('/').Last(),
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                new(),
                                default,
                                string.Empty,
                                default,
                                new(),
                                new(),
                                new()
                            );
                            lock (TrafficLight)
                            {
                                resource.Costs.AddRange(costForResourceNotMoreAvailablecostForResourceNotMoreAvailable);
                                outputResourceGroup.Resources.Add(resource);
                            }
                        };
                        lock (TrafficLight)
                            outputSubscription.ResourceGroups.Add(outputResourceGroup);
                    }
                }
                await Task.WhenAll(resourceGroupTasks).NoContext();
                return outputSubscription;
            }
            catch (Exception ex)
            {
                Errors.Add(new CloudManagementError(subscription.Id, subscription.DisplayName, CloudManagementErrorType.Subscription, ex));
                return default;
            }
        }
        public async Task<(Subscription Subscription, List<CloudManagementError> Errors)> GetSubscriptionAsync(string subscriptionId, DateTime startTime, DateTime endTime, ManagementDeepRequest azureDeepRequest, bool executeRequestInParallel)
        {
            List<AzureSubscription> subscriptions = (await (await GetAuthenticatedClientAsync().NoContext())
                .GetFromJsonAsync<AzureSubscriptions>($"https://management.azure.com/subscriptions?api-version=2020-01-01", Options).NoContext())
                .Subscriptions;
            AzureSubscription subscription = subscriptions.FirstOrDefault(x => x.SubscriptionId == subscriptionId);
            if (subscription != default)
                return (await GetSubscriptionAsync(subscription, startTime, endTime, azureDeepRequest, executeRequestInParallel).NoContext(), Errors);
            else
                return default;
        }
        private readonly ConcurrentDictionary<string, List<string>> PossibleMetricsByType = new();
        private const string WrongMetric = "a";
        private async Task GetPossibleMetricsAsync(Resource resource)
        {
            if (!PossibleMetricsByType.ContainsKey(resource.Type))
            {
                try
                {
                    await GetMetricAsync(resource, DateTime.UtcNow.AddDays(-10), DateTime.UtcNow, WrongMetric, string.Empty);
                }
                catch (Exception ex)
                {
                    string errorResponse = ex.ToString();
                    if (RegexToSplitValidMetrics.IsMatch(errorResponse))
                        PossibleMetricsByType.TryAdd(resource.Type, RegexToSplitValidMetrics.Split(errorResponse).Last().Split('.').First().Trim(')').Split(',').Select(x => x.Trim().Trim('}').Trim('"')).SkipLast(1).ToList());
                    else
                        PossibleMetricsByType.TryAdd(resource.Type, new());
                }
            }
            if (PossibleMetricsByType.ContainsKey(resource.Type))
                resource.PossibleMetrics.AddRange(PossibleMetricsByType[resource.Type]);
        }
        private async Task<List<AzureResourceGroup>> GetResourceGroupsAsync(string subscriptionId)
        {
            try
            {
                return (await (await GetAuthenticatedClientAsync().NoContext())
                        .GetFromJsonAsync<AzureResourceGroups>($"https://management.azure.com/subscriptions/{subscriptionId}/resourcegroups?api-version=2020-06-01", Options)
                            .NoContext())
                        .ResourceGroups;
            }
            catch (Exception ex)
            {
                Errors.Add(new CloudManagementError(subscriptionId, string.Empty, CloudManagementErrorType.ResourceGroup, ex));
                return default;
            }
        }

        private async Task<List<AzureResource>> GetResourcesAsync(string subscriptionId)
        {
            try
            {
                return (await (await GetAuthenticatedClientAsync().NoContext())
                            .GetFromJsonAsync<AzureResources>($"https://management.azure.com/subscriptions/{subscriptionId}/resources?api-version=2020-06-01", Options)
                        .NoContext()).Resources;
            }
            catch (Exception ex)
            {
                Errors.Add(new CloudManagementError(subscriptionId, string.Empty, CloudManagementErrorType.Resource, ex));
                return default;
            }
        }

        private async Task<List<Cost>> GetCostsAsync(string subscriptionId, DateTime startTime, DateTime endTime, List<AzureConsumption> azureConsumptions)
        {
            try
            {
                string body = $"{{\"type\":\"Usage\",\"dataSet\":{{\"granularity\":\"Monthly\",\"aggregation\":{{\"totalCost\":{{\"name\":\"Cost\",\"function\":\"Sum\"}},\"totalCostUSD\":{{\"name\":\"CostUSD\",\"function\":\"Sum\"}}}},\"sorting\":[{{\"direction\":\"ascending\",\"name\":\"BillingMonth\"}}],\"grouping\":[{{\"type\":\"Dimension\",\"name\":\"ResourceId\"}},{{\"type\":\"Dimension\",\"name\":\"ResourceType\"}},{{\"type\":\"Dimension\",\"name\":\"ResourceLocation\"}},{{\"type\":\"Dimension\",\"name\":\"ChargeType\"}},{{\"type\":\"Dimension\",\"name\":\"ResourceGroupName\"}},{{\"type\":\"Dimension\",\"name\":\"PublisherType\"}},{{\"type\":\"Dimension\",\"name\":\"ServiceName\"}},{{\"type\":\"Dimension\",\"name\":\"ServiceTier\"}},{{\"type\":\"Dimension\",\"name\":\"Meter\"}}]}},\"timeframe\":\"Custom\",\"timePeriod\":{{\"from\":\"{startTime:yyyy-MM-dd}T00:00:00+00:00\",\"to\":\"{endTime:yyyy-MM-dd}T23:59:59+00:00\"}}}}";
                return (await (await GetAuthenticatedClientAsync().NoContext())
                    .PostAsync<AzureCost>($"https://management.azure.com/subscriptions/{subscriptionId}/providers/Microsoft.CostManagement/query?api-version=2019-11-01",
                        new StringContent(body, Encoding.UTF8, "application/json"),
                        Options)
                        .NoContext())
                    .Properties.Rows.Select(consumption =>
                    {
                        consumption[2].TryGetDateTime(out DateTime eventTime);
                        consumption[0].TryGetDecimal(out decimal billed);
                        consumption[1].TryGetDecimal(out decimal usdBilled);
                        string resourceId = consumption[3].GetString().ToLower();
                        string subCategory = consumption[10].GetString().ToLower();
                        string meter = consumption[11].GetString().ToLower();
                        string startingLabelForProduct = $"{subCategory} - {meter}";
                        return new Cost(eventTime,
                            billed,
                            usdBilled,
                            resourceId,
                            consumption[7].GetString().ToLower(),
                            consumption[12].GetString().ToLower(),
                            consumption[9].GetString().ToLower(),
                            subCategory,
                            meter,
                            azureConsumptions
                                .Where(ƒ => ƒ.Properties != null && ƒ.Properties.ResourceId?.ToLower().Equals(resourceId) == true && ƒ.Properties.Product?.ToLower().StartsWith(startingLabelForProduct) == true)
                                .Select(ƒ => new Consumption(
                                    ƒ.Properties.BillingAccountId,
                                    ƒ.Properties.Quantity,
                                    ƒ.Properties.EffectivePrice,
                                    ƒ.Properties.Cost,
                                    ƒ.Properties.UnitPrice,
                                    ƒ.Properties.BillingCurrency,
                                    ƒ.Properties.OfferId,
                                    ƒ.Properties.ChargeType,
                                    ƒ.Properties.Frequency,
                                    ƒ.Properties.MeterDetails?.UnitOfMeasure ?? string.Empty,
                                    ƒ.Properties.Date))
                                .OrderBy(x => x.EventDate)
                                .ToList() ?? new()
                            );
                    }
                    ).ToList();
            }
            catch (Exception ex)
            {
                Errors.Add(new CloudManagementError(subscriptionId, string.Empty, CloudManagementErrorType.Cost, ex));
                return new();
            }
        }
        private async Task<List<AzureConsumption>> GetConsumptionsAsync(string subscriptionId, DateTime startTime, DateTime endTime)
        {
            List<AzureConsumption> consumptions = new();
            try
            {
                string uri = $"https://management.azure.com/subscriptions/{subscriptionId}/providers/Microsoft.Consumption/usageDetails?$filter=properties%2FusageStart%20ge%20'{startTime:yyyy-MM-dd}'%20and%20properties%2FusageEnd%20le%20'{endTime:yyyy-MM-dd}'&$top={1_000}&api-version=2019-10-01";
                while (!string.IsNullOrWhiteSpace(uri))
                {
                    var consumptionResponse = await (await GetAuthenticatedClientAsync().NoContext())
                        .GetFromJsonAsync<AzureConsumptions>(uri, Options)
                        .NoContext();
                    consumptions.AddRange(consumptionResponse.Value);
                    uri = consumptionResponse.NextLink;
                }
            }
            catch (Exception ex)
            {
                Errors.Add(new CloudManagementError(subscriptionId, string.Empty, CloudManagementErrorType.Consumption, ex));
            }
            return consumptions;
        }
        private static readonly List<string> Aggregations = new() { "average", "maximum" };
        private async Task<List<Monitoring>> GetMonitoringsAsync(Resource resource, DateTime startTime, DateTime endTime, string subscriptionId)
        {
            List<Monitoring> monitorings = new();
            foreach (var toMonitoring in resource.PossibleMetrics.Take(2))
            {
                foreach (var aggregation in Aggregations)
                {
                    try
                    {
                        monitorings.Add(new Monitoring(
                            toMonitoring,
                            aggregation,
                            await GetDatas(toMonitoring, aggregation, subscriptionId, resource.Id).NoContext()
                        ));
                    }
                    catch (Exception ex)
                    {
                        Errors.Add(new CloudManagementError(subscriptionId, resource.Id, CloudManagementErrorType.Metric, ex));
                    }
                }
            }
            return monitorings;

            async Task<List<Datum>> GetDatas(string metricName, string metricAggregation, string subscriptionId, string resourceId)
            {
                try
                {
                    return (await GetMetricAsync(resource, startTime, endTime, metricName, metricAggregation).NoContext())
                        .FromJson<AzureMonitoring>()
                        .Value
                        .SelectMany(x => x.Timeseries.SelectMany(x => x.Data))
                        .Select(x => new Datum(x.TimeStamp, x.Maximum > 0 ? x.Maximum : x.Average))
                        .ToList();
                }
                catch (Exception ex)
                {
                    Errors.Add(new CloudManagementError(subscriptionId, resourceId, CloudManagementErrorType.Metric, ex));
                    return new();
                }
            }
        }
        private async Task<string> GetMetricAsync(Resource resource, DateTime startTime, DateTime endTime, string metricName, string metricAggregation = "average") //"maximum"
            => await (await GetAuthenticatedClientAsync().NoContext())
            .GetStringAsync($"https://management.azure.com{resource.Id}/providers/microsoft.Insights/metrics?timespan={startTime:yyyy-MM-dd}T23:00:00.000Z/{endTime:yyyy-MM-dd}T23:00:00.000Z&interval=PT6H{(!string.IsNullOrWhiteSpace(metricName) ? $"&metricnames={metricName}" : string.Empty)}{(!string.IsNullOrWhiteSpace(metricAggregation) ? $"&aggregation={metricAggregation}" : string.Empty)}&metricNamespace={HttpUtility.UrlEncode(resource.Type)}&autoadjusttimegrain=true&validatedimensions=false&api-version=2019-07-01")
                .NoContext();
    }
}