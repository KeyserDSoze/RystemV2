using System.Collections.Generic;
using System.Linq;

namespace Rystem.Cloud
{
    public sealed record ResourceGroup(string Id, string Name, string Location, Dictionary<string, string> Tags, List<Resource> Resources)
    {
        public decimal Billed => Resources.Sum(x => x.Billed);
        public decimal UsdBilled => Resources.Sum(x => x.UsdBilled);
        public IEnumerable<string> Types => Resources.Select(x => x.Type).Distinct();
        public Dictionary<string, List<string>> PossibleMetrics
            => Resources.Where(x => x.Type != null).GroupBy(x => x.Type)
                    .ToDictionary(x => x.Key, x => x.FirstOrDefault().PossibleMetrics);
        public Dictionary<ConsumptionKey, List<Consumption>> GetConsumptions(bool withBillAccountAndOfferId = false, Dictionary<ConsumptionKey, List<Consumption>> consumptions = default)
        {
            if (consumptions == default)
                consumptions = new();
            foreach (var resource in Resources)
            {
                foreach (var cost in resource.Costs)
                {
                    foreach (var consumption in cost.Consumptions)
                    {
                        var key = new ConsumptionKey(cost.Category, cost.SubCategory, cost.Meter, withBillAccountAndOfferId ? consumption.BillAccountId : string.Empty, withBillAccountAndOfferId ? consumption.OfferId : string.Empty);
                        if (!consumptions.ContainsKey(key))
                        {
                            consumptions.Add(key, new());
                            consumptions[key].Add(consumption);
                        }
                        else
                        {
                            var actualConsumption = consumptions[key].FirstOrDefault(x => x.EventDate == consumption.EventDate);
                            if (actualConsumption == default)
                                consumptions[key].Add(consumption);
                            else
                            {
                                consumptions[key].Remove(actualConsumption);
                                actualConsumption += consumption;
                                consumptions[key].Add(actualConsumption);
                            }
                        }
                    }
                }
            }
            return consumptions;
        }
    }
}