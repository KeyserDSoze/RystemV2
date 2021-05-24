using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Concurrency
{
    public interface IDistributedConcurrencyKey
    {
        string Key { get; }
        RystemDistributedServiceProvider ConfigureDistributed();
        internal RystemDistributedServiceProvider BuildDistributed()
            => ConfigureDistributed().AddInstance(this.GetType());
    }
}