using System;

namespace Rystem.Cloud
{
    public sealed record Consumption(string BillAccountId, decimal Quantity, decimal EffectivePrice, decimal Cost, decimal UnitPrice, string Currency, string OfferId, string ChargeType, string Frequency, string UnitOfMeasure, DateTime EventDate)
    {
        public static Consumption operator +(Consumption consumptionA, Consumption consumptionB)
            => consumptionA with
            {
                Quantity = consumptionA.Quantity + consumptionB.Quantity,
                Cost = consumptionA.Cost + consumptionB.Cost
            };
    }
}