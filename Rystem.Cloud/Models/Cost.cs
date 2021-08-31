using System;
using System.Collections.Generic;

namespace Rystem.Cloud
{
    public record Cost(DateTime EventDate, decimal Billed, decimal UsdBilled, string ResourceId, string ResourceGroup, string Currency, string Category, string SubCategory, string Meter, List<Consumption> Consumptions);
    public record Consumption(string BillAccountId, decimal Quantity, decimal EffectivePrice, decimal Cost, decimal UnitPrice, string Currency, string OfferId, string ChargeType, string Frequency, DateTime EventDate)
    {
        public static Consumption operator +(Consumption consumptionA, Consumption consumptionB)
            => consumptionA with
            {
                Quantity = consumptionA.Quantity + consumptionB.Quantity,
                Cost = consumptionA.Cost + consumptionB.Cost
            };
    }
    public record ConsumptionKey(string Category, string Subcategory, string Meter, string BillAccountId, string OfferId);
}