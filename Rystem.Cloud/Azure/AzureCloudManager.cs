using System;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Web;
using Rystem;
using System.IO;
using System.Text.RegularExpressions;
using Rystem.Net;
using Rystem.Text;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.CompilerServices;

namespace Rystem.Cloud.Azure
{
    public record AzureAadAppRegistration(string ClientId, string ClientSecret, string TenantId);
    internal sealed class AzureCloudManager : ICloudManagement
    {
        private readonly AzureAadAppRegistration AppRegistration;
        private AzureAccount AzureAccount { get; set; }
        public AzureCloudManager(AzureAadAppRegistration appRegistration)
            => AppRegistration = appRegistration;
        private bool IsAuthenticated => AzureAccount != null && DateTime.UtcNow >= AzureAccount.StartTime && DateTime.UtcNow <= AzureAccount.EndTime;
        private static readonly JsonSerializerOptions Options = new()
        {
            NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
        };
        private async Task AuthenticateAsync()
        {
            string body = $"grant_type=client_credentials&client_id={AppRegistration.ClientId}&client_secret={AppRegistration.ClientSecret}&resource=https://management.azure.com/";
            AzureAccount = await new Uri($"https://login.microsoftonline.com/{AppRegistration.TenantId}/oauth2/token")
                .CreateHttpRequest()
                .SetTimeout(180_000)
                .WithMethod(HttpMethod.Post)
                .AddContentType("application/x-www-form-urlencoded")
                .AddBody(body, EncodingType.UTF8)
                .Build()
                .InvokeAsync<AzureAccount>(Options)
                .NoContext();
        }
        private async Task<string> GetAccessToken()
        {
            if (!IsAuthenticated)
                await AuthenticateAsync().NoContext();
            return AzureAccount.AccessToken;
        }
        private async Task<(string Name, string Value)> GetAuthHeaders()
            => ("Authorization", $"Bearer {await GetAccessToken().NoContext()}");
        public async Task<Tenant> GetTenantAsync(DateTime from, DateTime to, ManagementDeepRequest deepRequest, bool executeRequestInParallel)
            => new Tenant(string.Empty, await GetSubscriptionsAsync(from, to, deepRequest, executeRequestInParallel).NoContext());
        private static readonly Regex RegexToSplitValidMetrics = new("Valid metrics:");
        private static readonly object TrafficLight = new();
        public async Task<IEnumerable<Subscription>> ListSubscriptionsAsync()
            => (await new Uri($"https://management.azure.com/subscriptions?api-version=2020-01-01")
                .CreateHttpRequest()
                .SetTimeout(180_000)
                .AddToHeaders(await GetAuthHeaders().NoContext())
                .Build()
                .InvokeAsync<AzureSubscriptions>(Options)).Subscriptions.Select(x =>
                    new Subscription(x.SubscriptionId, x.TenantId, x.DisplayName, x.State, x.Tags, new())
                );
        private async Task<List<Subscription>> GetSubscriptionsAsync(DateTime startTime, DateTime endTime, ManagementDeepRequest azureDeepRequest, bool executeRequestInParallel)
        {
            List<AzureSubscription> subscriptions = (await new Uri($"https://management.azure.com/subscriptions?api-version=2020-01-01")
                .CreateHttpRequest()
                .SetTimeout(180_000)
                .AddToHeaders(await GetAuthHeaders().NoContext())
                .Build()
                .InvokeAsync<AzureSubscriptions>(Options)).Subscriptions;
            List<Subscription> outputSubscriptions = new();
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
                    lock (TrafficLight)
                        outputSubscriptions.Add(outputSubscription);
                }
            }
            await Task.WhenAll(subscriptionTasks).NoContext();
            return outputSubscriptions;
        }
        private async Task<Subscription> GetSubscriptionAsync(AzureSubscription subscription, DateTime startTime, DateTime endTime, ManagementDeepRequest azureDeepRequest, bool executeRequestInParallel)
        {
            string value = await new Uri($"https://management.azure.com{subscription.Id}/providers/Microsoft.CostManagement/exports?api-version=2020-06-01")
                   .CreateHttpRequest()
                   .SetTimeout(180_000)
                   .AddToHeaders(await GetAuthHeaders().NoContext())
                   .Build()
                   .InvokeAsync().NoContext();
            Subscription outputSubscription = new(subscription.Id, subscription.TenantId, subscription.DisplayName, subscription.State, subscription.Tags, new());
            List<AzureResourceGroup> resourceGroups = await GetResourceGroupsAsync(subscription.SubscriptionId);
            List<AzureResource> resources = await GetResourcesAsync(subscription.SubscriptionId);
            AzureConsumptions consumptions = await GetConsumptionsAsync(subscription.SubscriptionId, startTime, endTime).NoContext();
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
                                var monitorings = await GetMonitoringsAsync(resource, startTime, endTime).NoContext();
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
        public async Task<Subscription> GetSubscriptionAsync(string subscriptionId, DateTime startTime, DateTime endTime, ManagementDeepRequest azureDeepRequest, bool executeRequestInParallel)
        {
            List<AzureSubscription> subscriptions = (await new Uri($"https://management.azure.com/subscriptions?api-version=2020-01-01")
              .CreateHttpRequest()
              .SetTimeout(180_000)
              .AddToHeaders(await GetAuthHeaders().NoContext())
              .Build()
              .InvokeAsync<AzureSubscriptions>(Options)).Subscriptions;
            AzureSubscription subscription = subscriptions.FirstOrDefault(x => x.SubscriptionId == subscriptionId);
            if (subscription != default)
                return await GetSubscriptionAsync(subscription, startTime, endTime, azureDeepRequest, executeRequestInParallel).NoContext();
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
                catch (WebException ex)
                {
                    try
                    {
                        const string label = "is not a supported platform metric namespace";
                        using StreamReader streamReader = new(ex.Response.GetResponseStream());
                        var errorResponse = streamReader.ReadToEnd();
                        if (!errorResponse.ToLower().Contains(label))
                            PossibleMetricsByType.TryAdd(resource.Type, RegexToSplitValidMetrics.Split(errorResponse).Last().Split(',').Select(x => x.Trim().Trim('}').Trim('"')).ToList());
                    }
                    catch { }
                }
            }
            if (PossibleMetricsByType.ContainsKey(resource.Type))
                resource.PossibleMetrics.AddRange(PossibleMetricsByType[resource.Type]);
        }
        private async Task<List<AzureResourceGroup>> GetResourceGroupsAsync(string subscriptionId)
            => (await new Uri($"https://management.azure.com/subscriptions/{subscriptionId}/resourcegroups?api-version=2020-06-01")
                .CreateHttpRequest()
                .SetTimeout(180_000)
                .AddToHeaders(await GetAuthHeaders().NoContext())
                .Build()
                .InvokeAsync<AzureResourceGroups>(Options).NoContext()).ResourceGroups;
        private async Task<List<AzureResource>> GetResourcesAsync(string subscriptionId)
            => (await new Uri($"https://management.azure.com/subscriptions/{subscriptionId}/resources?api-version=2020-06-01")
                .CreateHttpRequest()
                .SetTimeout(180_000)
                .AddToHeaders(await GetAuthHeaders().NoContext())
                .Build()
                .InvokeAsync<AzureResources>(Options).NoContext()).Resources; //.Replace("$type", "type").FromDefaultJson<AzureResources>()
        private async Task<List<Cost>> GetCostsAsync(string subscriptionId, DateTime startTime, DateTime endTime, AzureConsumptions azureConsumptions)
        {
            string body = $"{{\"type\":\"Usage\",\"dataSet\":{{\"granularity\":\"Monthly\",\"aggregation\":{{\"totalCost\":{{\"name\":\"Cost\",\"function\":\"Sum\"}},\"totalCostUSD\":{{\"name\":\"CostUSD\",\"function\":\"Sum\"}}}},\"sorting\":[{{\"direction\":\"ascending\",\"name\":\"BillingMonth\"}}],\"grouping\":[{{\"type\":\"Dimension\",\"name\":\"ResourceId\"}},{{\"type\":\"Dimension\",\"name\":\"ResourceType\"}},{{\"type\":\"Dimension\",\"name\":\"ResourceLocation\"}},{{\"type\":\"Dimension\",\"name\":\"ChargeType\"}},{{\"type\":\"Dimension\",\"name\":\"ResourceGroupName\"}},{{\"type\":\"Dimension\",\"name\":\"PublisherType\"}},{{\"type\":\"Dimension\",\"name\":\"ServiceName\"}},{{\"type\":\"Dimension\",\"name\":\"ServiceTier\"}},{{\"type\":\"Dimension\",\"name\":\"Meter\"}}]}},\"timeframe\":\"Custom\",\"timePeriod\":{{\"from\":\"{startTime:yyyy-MM-dd}T00:00:00+00:00\",\"to\":\"{endTime:yyyy-MM-dd}T23:59:59+00:00\"}}}}";
            return (await new Uri($"https://management.azure.com/subscriptions/{subscriptionId}/providers/Microsoft.CostManagement/query?api-version=2019-11-01")
                .CreateHttpRequest()
                .SetTimeout(180_000)
                .WithMethod(HttpMethod.Post)
                .AddToHeaders(await GetAuthHeaders().NoContext())
                .AddContentType("application/json")
                .AddBody(body, EncodingType.UTF8)
                .Build()
                .InvokeAsync<AzureCost>(Options).NoContext())
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
                        azureConsumptions?.Value?
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
                                ƒ.Properties.Date))
                            .OrderBy(x => x.EventDate)
                            .ToList() ?? new()
                        );
                }
                ).ToList();
        }
        private async Task<AzureConsumptions> GetConsumptionsAsync(string subscriptionId, DateTime startTime, DateTime endTime)
        {
            return (await new Uri($"https://management.azure.com/subscriptions/{subscriptionId}/providers/Microsoft.Consumption/usageDetails?$filter=properties%2FusageStart%20ge%20'{startTime:yyyy-MM-dd}'%20and%20properties%2FusageEnd%20le%20'{endTime:yyyy-MM-dd}'&$top={int.MaxValue}&api-version=2019-10-01")
                .CreateHttpRequest()
                .SetTimeout(180_000)
                .WithMethod(HttpMethod.Get)
                .AddToHeaders(await GetAuthHeaders().NoContext())
                .Build()
                .InvokeAsync<AzureConsumptions>().NoContext());
        }
        private static readonly List<string> Aggregations = new() { "average", "maximum" };
        private async Task<List<Monitoring>> GetMonitoringsAsync(Resource resource, DateTime startTime, DateTime endTime)
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
                            await GetDatas(toMonitoring, aggregation).NoContext()
                        ));
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            return monitorings;

            async Task<List<Datum>> GetDatas(string metricName, string metricAggregation)
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
                catch
                {
                    return new();
                }
            }
        }
        private async Task<string> GetMetricAsync(Resource resource, DateTime startTime, DateTime endTime, string metricName, string metricAggregation = "average") //"maximum"
            => await new Uri($"https://management.azure.com{resource.Id}/providers/microsoft.Insights/metrics?timespan={startTime:yyyy-MM-dd}T23:00:00.000Z/{endTime:yyyy-MM-dd}T23:00:00.000Z&interval=PT6H{(!string.IsNullOrWhiteSpace(metricName) ? $"&metricnames={metricName}" : string.Empty)}{(!string.IsNullOrWhiteSpace(metricAggregation) ? $"&aggregation={metricAggregation}" : string.Empty)}&metricNamespace={HttpUtility.UrlEncode(resource.Type)}&autoadjusttimegrain=true&validatedimensions=false&api-version=2019-07-01")
                .CreateHttpRequest()
                .SetTimeout(180_000)
                .AddToHeaders(await GetAuthHeaders().NoContext())
                .Build()
                .InvokeAsync()
                .NoContext();
    }
}