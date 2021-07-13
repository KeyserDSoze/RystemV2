using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Background
{
    public interface IAggregation<T>
    {
        RystemAggregationServiceProvider ConfigureSequence();
        internal RystemAggregationServiceProvider BuildSequence()
            => ConfigureSequence().AddInstance(this.GetType());
    }
}