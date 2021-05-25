using Rystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Cloud
{
    public record Tenant(string Name, List<Subscription> Subscriptions)
    {
        public Dictionary<string, List<string>> PossibleMetrics => Subscriptions.SelectMany(x => x.PossibleMetrics).Where(x => x.Value.Count > 0).GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Where(t => t.Key == x.Key).FirstOrDefault().Value);
        public IEnumerable<string> Types => Subscriptions.SelectMany(x => x.Types).Distinct();
        public decimal Billed => Subscriptions.Sum(x => x.Billed);
        public decimal UsdBilled => Subscriptions.Sum(x => x.UsdBilled);
    }
}