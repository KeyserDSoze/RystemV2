using Rystem.Azure;
using Rystem.Business;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rystem.Concurrency
{
    internal sealed class DistributedManager<TKey> : IDistributedManager<TKey>
        where TKey : IDistributedConcurrencyKey
    {
        private readonly Dictionary<Installation, IDistributedImplementation> Implementations = new();
        private readonly Dictionary<Installation, ProvidedService> DistributedConfigurations;
        private bool MemoryIsActive { get; }
        private readonly AzureManager Manager;
        public DistributedManager(Options<IDistributedManager<TKey>> options, AzureManager manager)
        {
            DistributedConfigurations = options.Services;
            Manager = manager;
            foreach (var conf in DistributedConfigurations)
            {
                ProvidedService configuration = DistributedConfigurations[conf.Key];
                switch (configuration.Type)
                {
                    case ServiceProviderType.AzureBlockBlobStorage:
                        Implementations.Add(conf.Key, new BlobStorageImplementation(Manager.BlobStorage(configuration.Configurations, configuration.ServiceKey)));
                        break;
                    case ServiceProviderType.AzureRedisCache:
                        Implementations.Add(conf.Key, new RedisCacheImplementation(Manager.RedisCache(configuration.ServiceKey)));
                        break;
                    case ServiceProviderType.InMemory:
                        Implementations.Add(conf.Key, new MemoryImplementation());
                        break;
                    default:
                        throw new InvalidOperationException($"Wrong type installed {configuration.Type}");
                }
            }
        }
        public Task<RaceConditionResponse> RunUnderRaceConditionAsync(TKey key, Func<Task> task, Installation installation = Installation.Default, TimeSpan timeWindow = default)
            => task.RunUnderRaceConditionAsync(key.Key, timeWindow, Implementations[installation]);
        public Task<LockResponse> LockAsync(TKey key, Func<Task> task, Installation installation = Installation.Default)
            => task.LockAsync(key.Key, Implementations[installation]);
        public async Task<bool> WarmUpAsync()
        {
            List<Task> tasks = new();
            foreach (var configuration in DistributedConfigurations)
                tasks.Add(Implementations[configuration.Key].WarmUpAsync());
            await Task.WhenAll(tasks).NoContext();
            return true;
        }
    }
}