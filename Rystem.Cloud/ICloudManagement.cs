using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rystem.Cloud
{
    public interface ICloudManagement
    {
        Task<(Tenant Tenant, List<CloudManagementError> Errors)> GetTenantByMonthAsync(DateTime month, ManagementDeepRequest deepRequest, bool executeRequestInParallel);
        Task<(Tenant Tenant, List<CloudManagementError> Errors)> GetTenantAsync(DateTime from, DateTime to, ManagementDeepRequest deepRequest, bool executeRequestInParallel);
        Task<(Subscription Subscription, List<CloudManagementError> Errors)> GetSubscriptionAsync(string subscriptionId, DateTime from, DateTime to, ManagementDeepRequest deepRequest, bool executeRequestInParallel);
        Task<IEnumerable<Subscription>> ListSubscriptionsAsync();
    }
}