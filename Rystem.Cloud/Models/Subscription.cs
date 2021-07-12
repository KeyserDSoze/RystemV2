using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Cloud
{
    public record Subscription(string Id, string TenantId, string DisplayName, string State, Dictionary<string, string> Tags, List<ResourceGroup> ResourceGroups)
    {
        public decimal Billed => ResourceGroups.Sum(x => x.Billed);
        public decimal UsdBilled => ResourceGroups.Sum(x => x.UsdBilled);
        public IEnumerable<string> Types => ResourceGroups.SelectMany(x => x.Types).Distinct();
        public Dictionary<string, List<string>> PossibleMetrics => ResourceGroups.SelectMany(x => x.PossibleMetrics).Where(x => x.Value.Count > 0).GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Where(t => t.Key == x.Key).FirstOrDefault().Value);
    }
}