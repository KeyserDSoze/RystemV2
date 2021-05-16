using Rystem.Azure.Installation;
using Rystem.Azure.Integration.Cache;
using Rystem.Azure.Integration.Storage;
using Rystem.Concurrency;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.Business
{
    internal class CacheManager<TCacheKey, TCache>
        where TCacheKey : ICacheKey<TCache>
        where TCache : new()
    {
        private readonly IDictionary<Installation, ProvidedService> CacheConfiguration;
        private bool MemoryIsActive { get; }
        private bool CloudIsActive { get; }
        private ICacheImplementation<TCache> CloudImplementation;
        private static readonly object TrafficLight = new();
        private ICacheImplementation<TCache> Implementation()
        {
            if (CloudImplementation == default)
                lock (TrafficLight)
                    if (CloudImplementation == default)
                    {
                        ProvidedService configuration = CacheConfiguration[Installation.Inst00];
                        switch (configuration.Type)
                        {
                            case ServiceProviderType.AzureBlobStorage:
                                CloudImplementation = new InBlobStorage<TCache>(new BlobStorageIntegration(configuration.Configurations, AzureManager.Instance.Storages[configuration.ServiceKey]), configuration.Configurations.Name ?? "Cache");
                                break;
                            case ServiceProviderType.AzureTableStorage:
                                CloudImplementation = new InTableStorage<TCache>(new TableStorageIntegration(configuration.Configurations, AzureManager.Instance.Storages[configuration.ServiceKey]), configuration.Configurations.Name ?? "Cache");
                                break;
                            case ServiceProviderType.AzureRedisCache:
                                CloudImplementation = new InRedisCache<TCache>(new RedisCacheIntegration(AzureManager.Instance.RedisCaches[configuration.ServiceKey]), configuration.Configurations.Name ?? "Cache");
                                break;
                            default:
                                throw new InvalidOperationException($"Wrong type installed {configuration.Type}");
                        }
                    }
            return CloudImplementation;
        }
        public CacheManager(RystemCacheServiceProvider serviceProvider)
        {
            MemoryIsActive = serviceProvider.Services.ContainsKey(Installation.Default);
            CloudIsActive = serviceProvider.Services.ContainsKey(Installation.Inst00);
            CacheConfiguration = serviceProvider.Services.ToDictionary(x => x.Key, x => x.Value);
        }
        private async Task<TCache> InstanceWithoutConsistencyAsync(TCacheKey key, string keyString)
        {
            TCache cache = await key.FetchAsync().NoContext();
            if (MemoryIsActive)
                new Key(keyString).Update(cache, default);
            if (CloudIsActive)
                await Implementation().UpdateAsync(keyString, cache, default).NoContext();
            return cache;
        }
        public async Task<TCache> InstanceAsync(TCacheKey key, bool withConsistency)
        {
            string keyString = key.ToKeyString();
            if (MemoryIsActive)
                if (new Key(keyString).Exists<TCache>())
                    return new Key(keyString).Instance<TCache>();
            if (CloudIsActive)
            {
                CacheStatus<TCache> responseFromCloud = await Implementation().ExistsAsync(keyString).NoContext();
                if (responseFromCloud.IsOk)
                {
                    TCache cache = responseFromCloud.Cache != null ? responseFromCloud.Cache : await Implementation().InstanceAsync(keyString).NoContext();
                    if (MemoryIsActive)
                        new Key(keyString).Update(cache, default);
                    return cache;
                }
            }
            if (!withConsistency)
                return await InstanceWithoutConsistencyAsync(key, keyString).NoContext();
            else
            {
                TCache cache = default;
                await RaceCondition.RunAsync(async () => cache = await InstanceWithoutConsistencyAsync(key, keyString).NoContext(), keyString).NoContext();
                return cache;
            }
        }
        public async Task<bool> UpdateAsync(TCacheKey key, TCache value, TimeSpan expiringTime)
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
            if (CloudIsActive)
                result |= await Implementation().UpdateAsync(keyString, value, expiringTime).NoContext();
            return result;
        }
        public async Task<bool> ExistsAsync(TCacheKey key)
        {
            string keyString = key.ToKeyString();
            if (MemoryIsActive)
                return new Key(keyString).Exists<TCache>();
            else if (CloudIsActive)
                return (await Implementation().ExistsAsync(keyString).NoContext()).IsOk;
            return false;
        }
        public async Task<bool> DeleteAsync(TCacheKey key)
        {
            string keyString = key.ToKeyString();
            bool result = false;
            if (MemoryIsActive)
                result |= new Key(keyString).Remove<TCache>() != null;
            if (CloudIsActive)
                result |= await Implementation().DeleteAsync(keyString).NoContext();
            return result;
        }
        public async Task<IEnumerable<string>> ListAsync()
        {
            if (CloudIsActive)
                return await Implementation().ListAsync().NoContext();
            if (MemoryIsActive)
                return new Key(string.Empty).List<TCache>();
            return null;
        }
    }
}