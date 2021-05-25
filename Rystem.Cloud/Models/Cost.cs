using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Cloud
{
    public record Cost(DateTime EventDate, decimal Billed, decimal UsdBilled, string ResourceId, string ResourceGroup, string Currency, double Usage);
}