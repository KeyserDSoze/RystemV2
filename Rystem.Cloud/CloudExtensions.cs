using Rystem.Business;
using Rystem.Cloud;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace System
{
    public static class CloudExtensions
    {
        private static readonly Dictionary<string, Dictionary<Installation, ICloudManagement>> CloudManagers = new();
        private static readonly object Semaphore = new();
        private static ICloudManagement GetCloudManagement<TEntry>(this TEntry entry, Installation installation)
            where TEntry : ICloud
        {
            string type = entry.GetType().FullName;
            if (!CloudManagers.ContainsKey(type))
                lock (Semaphore)
                    if (!CloudManagers.ContainsKey(type))
                    {
                        CloudManagers.Add(type, new());
                        foreach (var value in entry.Configure().Values)
                            CloudManagers[type].Add(value.Key, new Rystem.Cloud.Azure.AzureCloudManager(value.Value));
                    }
            return CloudManagers[type][installation];
        }
        public static async Task<Tenant> GetTenantAsync<TEntry>(this TEntry entry, DateTime from, DateTime to, ManagementDeepRequest deepRequest = ManagementDeepRequest.Monitoring, bool executeRequestInParallel = false, Installation installation = Installation.Default)
            where TEntry : ICloud
            => await entry.GetCloudManagement(installation).GetTenantAsync(from, to, deepRequest, executeRequestInParallel).NoContext();
        public static async Task<Tenant> GetTenantAsync<TEntry>(this TEntry entry, DateTime month, ManagementDeepRequest deepRequest = ManagementDeepRequest.Monitoring, bool executeRequestInParallel = false, Installation installation = Installation.Default)
            where TEntry : ICloud
            => await entry.GetCloudManagement(installation).GetTenantAsync(new DateTime(month.Year, month.Month, 1), new DateTime(month.AddMonths(1).Year, month.AddMonths(1).Month, 1, 23, 59, 59).AddDays(-1), deepRequest, executeRequestInParallel).NoContext();

        public static async Task<Subscription> GetSubscriptionAsync<TEntry>(this TEntry entry, string subscriptionId, DateTime from, DateTime to, ManagementDeepRequest deepRequest = ManagementDeepRequest.Monitoring, bool executeRequestInParallel = false, Installation installation = Installation.Default)
            where TEntry : ICloud
            => await entry.GetCloudManagement(installation).GetSubscriptionAsync(subscriptionId, from, to, deepRequest, executeRequestInParallel).NoContext();
        public static async Task<Subscription> GetSubscriptionAsync<TEntry>(this TEntry entry, string subscriptionId, DateTime month, ManagementDeepRequest deepRequest = ManagementDeepRequest.Monitoring, bool executeRequestInParallel = false, Installation installation = Installation.Default)
            where TEntry : ICloud
            => await entry.GetCloudManagement(installation).GetSubscriptionAsync(subscriptionId, new DateTime(month.Year, month.Month, 1), new DateTime(month.AddMonths(1).Year, month.AddMonths(1).Month, 1, 23, 59, 59).AddDays(-1), deepRequest, executeRequestInParallel).NoContext();

        public static Task<Tenant> GetPreviousMonthTenantAsync<TEntry>(this TEntry entry, ManagementDeepRequest deepRequest = ManagementDeepRequest.Monitoring, bool executeRequestInParallel = false, Installation installation = Installation.Default)
            where TEntry : ICloud
            => entry.GetTenantAsync(DateTime.UtcNow.AddMonths(-1), deepRequest, executeRequestInParallel, installation);
        public static Task<IEnumerable<Subscription>> ListSubscriptionsAsync<TEntry>(this TEntry entry)
            where TEntry : ICloud
            => entry.ListSubscriptionsAsync();
    }
}