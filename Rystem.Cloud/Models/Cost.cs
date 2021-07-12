using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Cloud
{
    public record Cost(DateTime EventDate, decimal Billed, decimal UsdBilled, string ResourceId, string ResourceGroup, string Currency, List<Consumption> Consumptions);
    public record Consumption(string BillAccountId, decimal Quantity, decimal EffectivePrice, decimal Cost, decimal UnitPrice, string Currency, string OfferId, string ChargeType, string Frequency);
}