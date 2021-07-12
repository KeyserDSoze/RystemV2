using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Cloud
{
    public record Monitoring(string Name, string Aggregation, List<Datum> Datas)
    {
        public double Average => Datas.Count > 0 ? Datas.Sum(x => x.Value) / Datas.Count : 0;
    }
    public record Datum(DateTime Timestamp, double Value);
}