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
        public async Task<Tenant> GetTenantAsync(DateTime from, DateTime to, ManagementDeepRequest deepRequest)
            => new Tenant(string.Empty, await GetSubscriptionsAsync(from, to, deepRequest).NoContext());

        private static readonly Regex RegexToSplitValidMetrics = new("Valid metrics:");
        private async Task<List<Subscription>> GetSubscriptionsAsync(DateTime startTime, DateTime endTime, ManagementDeepRequest azureDeepRequest = ManagementDeepRequest.Subscription)
        {
            List<AzureSubscription> subscriptions = (await new Uri($"https://management.azure.com/subscriptions?api-version=2020-01-01")
                .CreateHttpRequest()
                .AddToHeaders(await GetAuthHeaders().NoContext())
                .Build()
                .InvokeAsync<AzureSubscriptions>(Options)).Subscriptions;
            List<Subscription> outputSubscriptions = new();
            foreach (var subscription in subscriptions)
            {
                Subscription outputSubscription = new(subscription.Id, subscription.TenantId, subscription.DisplayName, subscription.State, new());
                List<AzureResourceGroup> resourceGroups = await GetResourceGroupsAsync(subscription.SubscriptionId);
                List<AzureResource> resources = await GetResourcesAsync(subscription.SubscriptionId);
                List<Cost> costs = azureDeepRequest >= ManagementDeepRequest.Cost ? await GetCostsAsync(subscription.SubscriptionId, startTime, endTime) : new();
                foreach (var costByResourceGroupNotMoreAvailable in costs.Where(x => !resourceGroups.Any(t => t.Name.ToLower() == x.ResourceGroup.ToLower())).GroupBy(x => x.ResourceGroup))
                {
                    resourceGroups.Add(new AzureResourceGroup
                    {
                        Id = $"/subscriptions/{subscription.SubscriptionId}/resourceGroups/{costByResourceGroupNotMoreAvailable.Key}",
                        Name = costByResourceGroupNotMoreAvailable.Key,
                    });
                }
                foreach (AzureResourceGroup resourceGroup in resourceGroups)
                {
                    ResourceGroup outputResourceGroup = new(resourceGroup.Id, resourceGroup.Name, resourceGroup.Location, resourceGroup.Tags, new());
                    List<Resource> resourcesFromResourceGroup = resources
                        .Where(x => x.Id.ToLower().StartsWith($"/subscriptions/{subscription.SubscriptionId}/resourceGroups/{resourceGroup.Name}/".ToLower()))
                        .Select(x => new Resource(x.Id, x.Name, x.Type, x.Kind, x.Location, x.Tags, new Sku(x.Sku.Name, x.Sku.Tier, x.Sku.Capacity, x.Sku.Size, x.Sku.Family), x.ManagedBy, x.Plan != default ? new Plan(x.Plan.Name, x.Plan.PromotionCode, x.Plan.Product, x.Plan.Publisher) : default, new(), new(), new()))
                        .ToList();
                    List<Cost> costByResourceGroup = costs.Where(x => x.ResourceGroup.ToLower() == resourceGroup.Name.ToLower()).ToList();
                    foreach (Resource resource in resourcesFromResourceGroup)
                    {
                        if (azureDeepRequest >= ManagementDeepRequest.Monitoring)
                        {
                            await GetPossibleMetricsAsync(resource).NoContext();
                            resource.Monitorings.AddRange(await GetMonitoringsAsync(resource, startTime, endTime).NoContext());
                        }
                        resource.Costs.AddRange(costByResourceGroup.Where(x => x.ResourceId.ToLower() == resource.Id.ToLower()));
                        outputResourceGroup.Resources.Add(resource);
                    }
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
                        resource.Costs.AddRange(costForResourceNotMoreAvailablecostForResourceNotMoreAvailable);
                        outputResourceGroup.Resources.Add(resource);
                    };
                    outputSubscription.ResourceGroups.Add(outputResourceGroup);
                }
            }
            return outputSubscriptions;
        }
        private readonly ConcurrentDictionary<string, IEnumerable<string>> PossibleMetricsByType = new();
        private async Task GetPossibleMetricsAsync(Resource resource)
        {
            if (!PossibleMetricsByType.ContainsKey(resource.Type))
            {
                try
                {
                    await GetMetricAsync(resource, DateTime.UtcNow.AddDays(-10), DateTime.UtcNow, "aa", string.Empty);
                }
                catch (WebException ex)
                {
                    try
                    {
                        using StreamReader streamReader = new(ex.Response.GetResponseStream());
                        var errorResponse = streamReader.ReadToEnd();
                        if (!errorResponse.ToLower().Contains("is not a supported platform metric namespace"))
                            PossibleMetricsByType.TryAdd(resource.Type, RegexToSplitValidMetrics.Split(errorResponse).Last().Split(',').Select(x => x.Trim().Trim('}').Trim('"')));
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
                .AddToHeaders(await GetAuthHeaders().NoContext())
                .Build()
                .InvokeAsync<AzureResourceGroups>(Options).NoContext()).ResourceGroups;
        private async Task<List<AzureResource>> GetResourcesAsync(string subscriptionId)
            => (await new Uri($"https://management.azure.com/subscriptions/{subscriptionId}/resources?api-version=2020-06-01")
                .CreateHttpRequest()
                .AddToHeaders(await GetAuthHeaders().NoContext())
                .Build()
                .InvokeAsync<AzureResources>(Options).NoContext()).Resources; //.Replace("$type", "type").FromDefaultJson<AzureResources>()
        private async Task<List<Cost>> GetCostsAsync(string subscriptionId, DateTime startTime, DateTime endTime)
        {
            string body = $"{{\"type\":\"Usage\",\"dataSet\":{{\"granularity\":\"Monthly\",\"aggregation\":{{\"totalCost\":{{\"name\":\"Cost\",\"function\":\"Sum\"}},\"totalCostUSD\":{{\"name\":\"CostUSD\",\"function\":\"Sum\"}}}},\"sorting\":[{{\"direction\":\"ascending\",\"name\":\"BillingMonth\"}}],\"grouping\":[{{\"type\":\"Dimension\",\"name\":\"ResourceId\"}},{{\"type\":\"Dimension\",\"name\":\"ResourceType\"}},{{\"type\":\"Dimension\",\"name\":\"ResourceLocation\"}},{{\"type\":\"Dimension\",\"name\":\"ChargeType\"}},{{\"type\":\"Dimension\",\"name\":\"ResourceGroupName\"}},{{\"type\":\"Dimension\",\"name\":\"PublisherType\"}},{{\"type\":\"Dimension\",\"name\":\"ServiceName\"}},{{\"type\":\"Dimension\",\"name\":\"ServiceTier\"}},{{\"type\":\"Dimension\",\"name\":\"Meter\"}}]}},\"timeframe\":\"Custom\",\"timePeriod\":{{\"from\":\"{startTime:yyyy-MM-dd}T00:00:00+00:00\",\"to\":\"{endTime:yyyy-MM-dd}T23:59:59+00:00\"}}}}";
            return (await new Uri($"https://management.azure.com/subscriptions/{subscriptionId}/providers/Microsoft.CostManagement/query?api-version=2019-11-01")
                .CreateHttpRequest()
                .WithMethod(HttpMethod.Post)
                .AddToHeaders(await GetAuthHeaders().NoContext())
                .AddContentType("application/json")
                .AddBody(body, EncodingType.UTF8)
                .Build()
                .InvokeAsync<AzureCost>(Options).NoContext())
                .Properties.Rows.Select(x =>
                {
                    x[2].TryGetDateTime(out DateTime eventTime);
                    x[0].TryGetDecimal(out decimal billed);
                    x[1].TryGetDecimal(out decimal usdBilled);
                    return new Cost(eventTime,
                        billed,
                        usdBilled,
                        x[3].GetString(),
                        x[7].GetString(),
                        x[12].GetString(),
                        0
                        );
                }
                ).ToList();
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
                        string elif = ex.ToString();
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
                        .Select(x => new Datum(x.TimeStamp, x.Average))
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
                .AddToHeaders(await GetAuthHeaders().NoContext())
                .Build()
                .InvokeAsync()
                .NoContext();
    }
}