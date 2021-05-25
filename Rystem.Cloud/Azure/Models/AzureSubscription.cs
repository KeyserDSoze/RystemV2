using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Rystem.Cloud.Azure
{
    internal sealed class AzureSubscriptions
    {
        [JsonPropertyName("value")]
        public List<AzureSubscription> Subscriptions { get; set; }
    }
    internal sealed class AzureSubscription
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("authorizationSource")]
        public string AuthorizationSource { get; set; }
        [JsonPropertyName("managedByTenants")]
        public object[] ManagedByTenants { get; set; }
        [JsonPropertyName("subscriptionId")]
        public string SubscriptionId { get; set; }
        [JsonPropertyName("tenantId")]
        public string TenantId { get; set; }
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }
        [JsonPropertyName("state")]
        public string State { get; set; }
        [JsonPropertyName("subscriptionPolicies")]
        public AzureSubscriptionPolicies SubscriptionPolicies { get; set; }
    }
    internal sealed class AzureSubscriptionPolicies
    {
        [JsonPropertyName("locationPlacementId")]
        public string LocationPlacementId { get; set; }
        [JsonPropertyName("quotaId")]
        public string QuotaId { get; set; }
        [JsonPropertyName("spendingLimit")]
        public string SpendingLimit { get; set; }
    }
}
