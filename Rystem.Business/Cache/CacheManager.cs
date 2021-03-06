using Rystem.Azure;
using Rystem.Concurrency;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rystem.Business
{
    internal sealed class CacheManager<TCacheKey, TCache> : ICacheManager<TCacheKey, TCache>
        where TCacheKey : ICacheKey<TCache>
    {
        private readonly Dictionary<Installation, ICacheImplementation<TCache>> Implementations = new();
        private readonly Dictionary<Installation, ProvidedService> CacheConfigurations;
        private bool MemoryIsActive { get; }
        private TimeSpan GetCorrectTimespan(TimeSpan expiringTime, Installation installation)
        {
            var option = CacheConfigurations[installation].Options as CacheConfiguration;
            if (expiringTime == default && option?.ExpiringDefault != default)
                expiringTime = option.ExpiringDefault;
            return expiringTime;
        }
        private readonly string Name;
        private readonly AzureManager Manager;
        public CacheManager(Options<ICacheManager<TCacheKey, TCache>> options, AzureManager manager)
        {
            Manager = manager;
            CacheConfigurations = options.Services;
            Name = typeof(TCache).Name;
            foreach(var conf in CacheConfigurations)
            {
                ProvidedService configuration = CacheConfigurations[conf.Key];
                switch (configuration.Type)
                {
                    case ServiceProviderType.AzureBlockBlobStorage:
                        Implementations.Add(conf.Key, new InBlobStorage<TCache>(Manager.BlobStorage(configuration.Configurations, configuration.ServiceKey), Name));
                        break;
                    case ServiceProviderType.AzureTableStorage:
                        Implementations.Add(conf.Key, new InTableStorage<TCache>(Manager.TableStorage(configuration.Configurations, configuration.ServiceKey), Name));
                        break;
                    case ServiceProviderType.AzureRedisCache:
                        Implementations.Add(conf.Key, new InRedisCache<TCache>(Manager.RedisCache(configuration.ServiceKey), configuration.Configurations.Name ?? "Cache"));
                        break;
                    case ServiceProviderType.InMemory:
                        MemoryIsActive = true;
                        break;
                    default:
                        throw new InvalidOperationException($"Wrong type installed {configuration.Type}");
                }
            }
        }
        private bool GetCloudIsActive(Installation installation)
            => CacheConfigurations.ContainsKey(installation) && CacheConfigurations[installation].Type != ServiceProviderType.InMemory;
        private async Task<TCache> InstanceWithoutConsistencyAsync(TCacheKey key, string keyString, TimeSpan expiringTime, Installation installation)
        {
            TCache cache = await key.FetchAsync().NoContext();
            if (MemoryIsActive)
                new Key(keyString).Update(cache, GetCorrectTimespan(expiringTime, Installation.Memory));
            if (GetCloudIsActive(installation))
                await Implementations[installation].UpdateAsync(keyString, cache, GetCorrectTimespan(expiringTime, installation)).NoContext();
            return cache;
        }
        public async Task<TCache> InstanceAsync(TCacheKey key, bool withConsistency, TimeSpan expiringTime, Installation installation)
        {
            string keyString = key.ToKeyString();
            if (MemoryIsActive)
                if (new Key(keyString).Exists<TCache>())
                    return new Key(keyString).Instance<TCache>();
            if (GetCloudIsActive(installation))
            {
                CacheStatus<TCache> responseFromCloud = await Implementations[installation].ExistsAsync(keyString).NoContext();
                if (responseFromCloud.IsOk)
                {
                    TCache cache = responseFromCloud.Cache != null ? responseFromCloud.Cache : await Implementations[installation].InstanceAsync(keyString).NoContext();
                    if (MemoryIsActive)
                        new Key(keyString).Update(cache, default);
                    return cache;
                }
            }
            if (!withConsistency)
                return await InstanceWithoutConsistencyAsync(key, keyString, expiringTime, installation).NoContext();
            else
            {
                TCache cache = default;
                await RaceCondition.RunAsync(async () => cache = await InstanceWithoutConsistencyAsync(key, keyString, expiringTime, installation).NoContext(), keyString).NoContext();
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
                new Key(keyString).Update(value, GetCorrectTimespan(expiringTime, Installation.Memory));
                result = true;
            }
            if (GetCloudIsActive(installation))
                result |= await Implementations[installation].UpdateAsync(keyString, value, GetCorrectTimespan(expiringTime, installation)).NoContext();
            return result;
        }
        public async Task<bool> ExistsAsync(TCacheKey key, Installation installation)
        {
            string keyString = key.ToKeyString();
            if (MemoryIsActive)
                return new Key(keyString).Exists<TCache>();
            else if (GetCloudIsActive(installation))
                return (await Implementations[installation].ExistsAsync(keyString).NoContext()).IsOk;
            return false;
        }
        public async Task<bool> DeleteAsync(TCacheKey key, Installation installation)
        {
            string keyString = key.ToKeyString();
            bool result = false;
            if (MemoryIsActive)
                result |= new Key(keyString).Remove<TCache>() != null;
            if (GetCloudIsActive(installation))
                result |= await Implementations[installation].DeleteAsync(keyString).NoContext();
            return result;
        }
        public async Task<IEnumerable<string>> ListAsync(Installation installation)
        {
            if (GetCloudIsActive(installation))
                return await Implementations[installation].ListAsync().NoContext();
            if (MemoryIsActive)
                return new Key(string.Empty).List<TCache>();
            return null;
        }
        public async Task<bool> WarmUpAsync()
        {
            List<Task> tasks = new();
            foreach (var configuration in CacheConfigurations)
                tasks.Add(Implementations[configuration.Key].WarmUpAsync());
            await Task.WhenAll(tasks).NoContext();
            return true;
        }
    }
}