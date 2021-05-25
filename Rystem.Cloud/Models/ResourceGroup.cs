using System.Collections.Generic;
using System.Linq;

namespace Rystem.Cloud
{
    public record ResourceGroup(string Id, string Name, string Location, Dictionary<string, string> Tags, List<Resource> Resources)
    {
        public decimal Billed => Resources.Sum(x => x.Billed);
        public decimal UsdBilled => Resources.Sum(x => x.UsdBilled);
        public IEnumerable<string> Types => Resources.Select(x => x.Type).Distinct();
        public Dictionary<string, List<string>> PossibleMetrics => Resources.Where(x => x.Type != null).GroupBy(x => x.Type).ToDictionary(x => x.Key, x => x.FirstOrDefault().PossibleMetrics);
    }
}