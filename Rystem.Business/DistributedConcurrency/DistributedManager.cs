using Rystem.Azure;
using Rystem.Business;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rystem.Concurrency
{
    internal class DistributedManager<TKey> : IDistributedManager<TKey>
        where TKey : IDistributedConcurrencyKey
    {
        private readonly Dictionary<Installation, IDistributedImplementation> Implementations = new();
        private readonly Dictionary<Installation, ProvidedService> DistributedConfigurations;
        private bool MemoryIsActive { get; }
        private readonly object TrafficLight = new();
        public IDistributedImplementation Implementation(Installation installation)
        {
            if (!Implementations.ContainsKey(installation))
                lock (TrafficLight)
                    if (!Implementations.ContainsKey(installation))
                    {
                        ProvidedService configuration = DistributedConfigurations[installation];
                        switch (configuration.Type)
                        {
                            case ServiceProviderType.AzureBlockBlobStorage:
                                Implementations.Add(installation, new BlobStorageImplementation(Manager.BlobStorage(configuration.Configurations, configuration.ServiceKey)));
                                break;
                            case ServiceProviderType.AzureRedisCache:
                                Implementations.Add(installation, new RedisCacheImplementation(Manager.RedisCache(configuration.ServiceKey)));
                                break;
                            default:
                                throw new InvalidOperationException($"Wrong type installed {configuration.Type}");
                        }
                    }
            return Implementations[installation];
        }
        private readonly AzureManager Manager;
        public DistributedManager(Options<IDistributedManager<TKey>> options, AzureManager manager)
        {
            DistributedConfigurations = options.Services;
            Manager = manager;
        }
        public Task<RaceConditionResponse> RunUnderRaceConditionAsync(TKey key, Func<Task> task, Installation installation = Installation.Default, TimeSpan timeWindow = default)
            => task.RunUnderRaceConditionAsync(key.Key, timeWindow, Implementation(installation));
        public Task<LockResponse> LockAsync(TKey key, Func<Task> task, Installation installation = Installation.Default)
            => task.LockAsync(key.Key, Implementation(installation));
        public async Task<bool> WarmUpAsync()
        {
            List<Task> tasks = new();
            foreach (var configuration in DistributedConfigurations)
                tasks.Add(Implementation(configuration.Key).WarmUpAsync());
            await Task.WhenAll(tasks).NoContext();
            return true;
        }
    }
}