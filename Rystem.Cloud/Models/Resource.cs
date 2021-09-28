using System.Collections.Generic;
using System.Linq;

namespace Rystem.Cloud
{
    public sealed record Resource(string Id, string Name, string Type, string Kind, string Location, Dictionary<string, string> Tags,
        Sku Sku, string ManagedBy, Plan Plan, List<Cost> Costs, List<Monitoring> Monitorings, List<string> PossibleMetrics)
    {
        public decimal Billed => Costs?.Sum(x => x.Billed) ?? 0;
        public decimal UsdBilled => Costs?.Sum(x => x.UsdBilled) ?? 0;
    }
}