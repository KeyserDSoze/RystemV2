using Rystem.Business;
using Rystem.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.UnitTest.Business.DistributedConcurrency
{
    internal sealed class DealingKey : IDistributedConcurrencyKey
    {
        public string Key { get; init; }
        public RystemDistributedServiceProvider ConfigureDistributed()
        {
            return RystemDistributedServiceProvider
                .WithAzure()
                .WithBlobStorage()
                .AndWithAzure(Installation.Inst00)
                .WithRedisCache();
        }
    }
}