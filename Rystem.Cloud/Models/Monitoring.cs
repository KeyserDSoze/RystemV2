using System.Collections.Generic;
using System.Linq;

namespace Rystem.Cloud
{
    public sealed record Monitoring(string Name, string Aggregation, List<Datum> Datas)
    {
        public double Average => Datas.Count > 0 ? Datas.Sum(x => x.Value) / Datas.Count : 0;
    }
}