using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Rystem.Cloud
{
    public record Resource(string Id, string Name, string Type, string Kind, string Location, Dictionary<string, string> Tags,
        Sku Sku, string ManagedBy, Plan Plan, List<Cost> Costs, List<Monitoring> Monitorings, List<string> PossibleMetrics)
    {
        public decimal Billed => Costs?.Sum(x => x.Billed) ?? 0;
        public decimal UsdBilled => Costs?.Sum(x => x.UsdBilled) ?? 0;
    }

    public record Sku(string Name, string Tier, int Capacity, string Size, string Family);
    public record Plan(string Name, string PromotionCode, string Product, string Publisher);
}