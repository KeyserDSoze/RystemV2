using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rystem.Cloud
{
    internal interface ICloudManagement
    {
        Task<Tenant> GetTenantAsync(DateTime from, DateTime to, ManagementDeepRequest deepRequest, bool multiTasking);
        Task<Subscription> GetSubscriptionAsync(string subscriptionId, DateTime from, DateTime to, ManagementDeepRequest deepRequest);
        Task<IEnumerable<Subscription>> ListSubscriptionsAsync();
    }
}
