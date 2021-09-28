using System;
using System.Collections.Generic;

namespace Rystem.Cloud
{
    public sealed record Cost(DateTime EventDate, decimal Billed, decimal UsdBilled, string ResourceId, string ResourceGroup, string Currency, string Category, string SubCategory, string Meter, List<Consumption> Consumptions);
}