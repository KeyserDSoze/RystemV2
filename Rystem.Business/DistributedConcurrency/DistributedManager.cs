using Rystem.Azure.Installation;
using Rystem.Azure.Integration.Cache;
using Rystem.Azure.Integration.Storage;
using Rystem.Business;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.Concurrency
{
    internal class DistributedManager<TKey>
        where TKey : IDistributedConcurrencyKey
    {
        private readonly Dictionary<Installation, IDistributedImplementation> Implementations = new();
        private readonly Dictionary<Installation, ProvidedService> DistributedConfiguration;
        private bool MemoryIsActive { get; }
        private static readonly object TrafficLight = new();
        public IDistributedImplementation Implementation(Installation installation)
        {
            if (!Implementations.ContainsKey(installation))
                lock (TrafficLight)
                    if (!Implementations.ContainsKey(installation))
                    {
                        ProvidedService configuration = DistributedConfiguration[installation];
                        switch (configuration.Type)
                        {
                            case ServiceProviderType.AzureBlockBlobStorage:
                                Implementations.Add(installation, new BlobStorageImplementation(AzureBuilder.Factory.BlobStorage(configuration.Configurations, configuration.ServiceKey)));
                                break;
                            case ServiceProviderType.AzureRedisCache:
                                Implementations.Add(installation, new RedisCacheImplementation(AzureBuilder.Factory.RedisCache(configuration.ServiceKey)));
                                break;
                            default:
                                throw new InvalidOperationException($"Wrong type installed {configuration.Type}");
                        }
                    }
            return Implementations[installation];
        }
        public DistributedManager(RystemDistributedServiceProvider serviceProvider)
            => DistributedConfiguration = serviceProvider.Services.ToDictionary(x => x.Key, x => x.Value);
    }
}