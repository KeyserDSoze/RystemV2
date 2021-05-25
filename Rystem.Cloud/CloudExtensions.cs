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
        public static async Task<Tenant> GetTenantAsync<TEntry>(this TEntry entry, DateTime from, DateTime to, ManagementDeepRequest deepRequest = ManagementDeepRequest.Monitoring, Installation installation = Installation.Default)
            where TEntry : ICloud
            => await entry.GetCloudManagement(installation).GetTenantAsync(from, to, deepRequest).NoContext();
    }
}