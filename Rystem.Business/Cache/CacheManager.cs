using Rystem.Azure.Installation;
using Rystem.Azure.Integration.Cache;
using Rystem.Azure.Integration.Storage;
using Rystem.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rystem.Business
{
    internal class CacheManager<TCacheKey, TCache>
        where TCacheKey : ICacheKey<TCache>
        where TCache : new()
    {
        private readonly Dictionary<Installation, ICacheImplementation<TCache>> Implementations = new();
        private readonly Dictionary<Installation, ProvidedService> CacheConfiguration;
        private bool MemoryIsActive { get; }
        private static readonly object TrafficLight = new();
        private ICacheImplementation<TCache> Implementation(Installation installation)
        {
            if (!Implementations.ContainsKey(installation))
                lock (TrafficLight)
                    if (!Implementations.ContainsKey(installation))
                    {
                        ProvidedService configuration = CacheConfiguration[installation];
                        switch (configuration.Type)
                        {
                            case ServiceProviderType.AzureBlockBlobStorage:
                                Implementations.Add(installation, new InBlobStorage<TCache>(new BlobStorageIntegration(configuration.Configurations, AzureManager.Instance.Storages[configuration.ServiceKey]), configuration.Configurations.Name ?? "Cache"));
                                break;
                            case ServiceProviderType.AzureTableStorage:
                                Implementations.Add(installation, new InTableStorage<TCache>(new TableStorageIntegration(configuration.Configurations, AzureManager.Instance.Storages[configuration.ServiceKey]), configuration.Configurations.Name ?? "Cache"));
                                break;
                            case ServiceProviderType.AzureRedisCache:
                                Implementations.Add(installation, new InRedisCache<TCache>(new RedisCacheIntegration(AzureManager.Instance.RedisCaches[configuration.ServiceKey]), configuration.Configurations.Name ?? "Cache"));
                                break;
                            default:
                                throw new InvalidOperationException($"Wrong type installed {configuration.Type}");
                        }
                    }
            return Implementations[installation];
        }
        public CacheManager(RystemCacheServiceProvider serviceProvider)
        {
            MemoryIsActive = serviceProvider.Services.ContainsKey(Installation.Memory);
            CacheConfiguration = serviceProvider.Services.ToDictionary(x => x.Key, x => x.Value);
        }
        private bool GetCloudIsActive(Installation installation)
            => CacheConfiguration.ContainsKey(installation) && CacheConfiguration[installation].Type != ServiceProviderType.InMemory;
        private async Task<TCache> InstanceWithoutConsistencyAsync(TCacheKey key, string keyString, Installation installation)
        {
            TCache cache = await key.FetchAsync().NoContext();
            if (MemoryIsActive)
                new Key(keyString).Update(cache, default);
            if (GetCloudIsActive(installation))
                await Implementation(installation).UpdateAsync(keyString, cache, default).NoContext();
            return cache;
        }
        public async Task<TCache> InstanceAsync(TCacheKey key, bool withConsistency, Installation installation)
        {
            string keyString = key.ToKeyString();
            if (MemoryIsActive)
                if (new Key(keyString).Exists<TCache>())
                    return new Key(keyString).Instance<TCache>();
            if (GetCloudIsActive(installation))
            {
                CacheStatus<TCache> responseFromCloud = await Implementation(installation).ExistsAsync(keyString).NoContext();
                if (responseFromCloud.IsOk)
                {
                    TCache cache = responseFromCloud.Cache != null ? responseFromCloud.Cache : await Implementation(installation).InstanceAsync(keyString).NoContext();
                    if (MemoryIsActive)
                        new Key(keyString).Update(cache, default);
                    return cache;
                }
            }
            if (!withConsistency)
                return await InstanceWithoutConsistencyAsync(key, keyString, installation).NoContext();
            else
            {
                TCache cache = default;
                await RaceCondition.RunAsync(async () => cache = await InstanceWithoutConsistencyAsync(key, keyString, installation).NoContext(), keyString).NoContext();
                return cache;
            }
        }
        public async Task<bool> UpdateAsync(TCacheKey key, TCache value, TimeSpan expiringTime, Installation installation)
        {
            string keyString = key.ToKeyString();
            if (value == null)
                value = await key.FetchAsync().NoContext();
            bool result = false;
            if (MemoryIsActive)
            {
                new Key(keyString).Update(value, expiringTime);
                result = true;
            }
            if (GetCloudIsActive(installation))
                result |= await Implementation(installation).UpdateAsync(keyString, value, expiringTime).NoContext();
            return result;
        }
        public async Task<bool> ExistsAsync(TCacheKey key, Installation installation)
        {
            string keyString = key.ToKeyString();
            if (MemoryIsActive)
                return new Key(keyString).Exists<TCache>();
            else if (GetCloudIsActive(installation))
                return (await Implementation(installation).ExistsAsync(keyString).NoContext()).IsOk;
            return false;
        }
        public async Task<bool> DeleteAsync(TCacheKey key, Installation installation)
        {
            string keyString = key.ToKeyString();
            bool result = false;
            if (MemoryIsActive)
                result |= new Key(keyString).Remove<TCache>() != null;
            if (GetCloudIsActive(installation))
                result |= await Implementation(installation).DeleteAsync(keyString).NoContext();
            return result;
        }
        public async Task<IEnumerable<string>> ListAsync(Installation installation)
        {
            if (GetCloudIsActive(installation))
                return await Implementation(installation).ListAsync().NoContext();
            if (MemoryIsActive)
                return new Key(string.Empty).List<TCache>();
            return null;
        }
    }
}