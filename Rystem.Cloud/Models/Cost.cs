using System;
using System.Collections.Generic;

namespace Rystem.Cloud
{
    public record Cost(DateTime EventDate, decimal Billed, decimal UsdBilled, string ResourceId, string ResourceGroup, string Currency, string Category, string SubCategory, string Meter, List<Consumption> Consumptions);
    public record Consumption(string BillAccountId, decimal Quantity, decimal EffectivePrice, decimal Cost, decimal UnitPrice, string Currency, string OfferId, string ChargeType, string Frequency, DateTime EventDate);
}