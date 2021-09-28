using System.Collections.Generic;
using System.Linq;

namespace Rystem.Cloud
{
    public sealed record Subscription(string Id, string TenantId, string DisplayName, string State, Dictionary<string, string> Tags, List<ResourceGroup> ResourceGroups)
    {
        public decimal Billed => ResourceGroups.Sum(x => x.Billed);
        public decimal UsdBilled => ResourceGroups.Sum(x => x.UsdBilled);
        public IEnumerable<string> Types => ResourceGroups.SelectMany(x => x.Types).Distinct();
        public Dictionary<string, List<string>> PossibleMetrics => ResourceGroups.SelectMany(x => x.PossibleMetrics).Where(x => x.Value.Count > 0).GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Where(t => t.Key == x.Key).FirstOrDefault().Value);
        public Dictionary<ConsumptionKey, List<Consumption>> GetConsumptions(bool withBillAccountAndOfferId = false, Dictionary<ConsumptionKey, List<Consumption>> consumptions = default)
        {
            if (consumptions == default)
                consumptions = new();
            foreach (var resourceGroup in ResourceGroups)
                _ = resourceGroup.GetConsumptions(withBillAccountAndOfferId, consumptions);
            return consumptions;
        }
    }
}