using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rystem.Cloud
{
    internal interface ICloudManagement
    {
        Task<Tenant> GetTenantAsync(DateTime from, DateTime to, ManagementDeepRequest deepRequest, bool executeRequestInParallel);
        Task<Subscription> GetSubscriptionAsync(string subscriptionId, DateTime from, DateTime to, ManagementDeepRequest deepRequest, bool executeRequestInParallel);
        Task<IEnumerable<Subscription>> ListSubscriptionsAsync();
        List<CloudManagementError> Errors { get; }
    }
}