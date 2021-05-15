using Rystem.Azure.Installation;
using Rystem.Azure.Integration.Cache;
using Rystem.Azure.Integration.Storage;
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
                                CloudImplementation = new InBlobStorage<TCache>(new BlobStorageIntegration(configuration.Configurations, AzureManager.Instance.Storages[configuration.ServiceKey]));
                                break;
                            case ServiceProviderType.AzureTableStorage:
                                CloudImplementation = new InTableStorage<TCache>(new TableStorageIntegration(configuration.Configurations, AzureManager.Instance.Storages[configuration.ServiceKey]));
                                break;
                            case ServiceProviderType.AzureRedisCache:
                                CloudImplementation = new InRedisCache<TCache>(new RedisCacheIntegration(AzureManager.Instance.RedisCaches[configuration.ServiceKey]));
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

        private static readonly ConcurrentDictionary<string, MyLazy<PromisedCache>> Promised = new();
        private class MyLazy<TEntity>
        {
            private readonly Func<TEntity> Creator;
            private TEntity value;
            private static readonly object TrafficLight = new();
            public TEntity Value
            {
                get
                {
                    if (value == null)
                        lock (TrafficLight)
                            if (value == null)
                                return value = Creator.Invoke();
                    return value;
                }
            }

            public MyLazy(Func<TEntity> creator)
            {
                this.Creator = creator;
            }
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
        private async Task<TCache> InstanceWithConsistencyAsync(TCacheKey key, string keyString)
        {
            try
            {
                TCache cache = default;
                Promised.TryAdd(keyString,
                    new MyLazy<PromisedCache>(() => new PromisedCache(new Instancer(key, keyString, Implementation()))));
                Promised.TryGetValue(keyString, out MyLazy<PromisedCache> lazy);
                if (lazy != null)
                {
                    PromisedCache promisedCache = lazy.Value;
                    PromisedState promisedState = default;
                    while (!(promisedState = promisedCache.Run()).IsCompleted())
                        await Task.Delay(100).NoContext();
                    if (promisedState.HasThrownAnException())
                        throw promisedState.Exception;
                    if (promisedState.HasEmptyResponse())
                        return cache;
                    cache = promisedCache.Cache;
                    if (cache == null)
                        return cache;
                    if (MemoryIsActive)
                        new Key(keyString).Update(cache, default);
                }
                if (cache != null)
                    return cache;
                else if (MemoryIsActive)
                    return new Key(keyString).Instance<TCache>();
                else
                    return await Implementation().InstanceAsync(keyString).NoContext();
            }
            finally
            {
                Promised.TryRemove(keyString, out _);
            }
        }
        public async Task<TCache> InstanceAsync(TCacheKey key)
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
            if (this.Configuration.Consistency == CacheConsistency.Never)
                return await InstanceWithoutConsistencyAsync(key, keyString).NoContext();
            else
                return await InstanceWithConsistencyAsync(key, keyString).NoContext();
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
        public async Task WarmUp()
            => await Implementation().WarmUp().NoContext();
        private class Instancer
        {
            public ICacheKey<TCache> Key { get; }
            public string KeyString { get; }
            public bool CloudIsActive { get; }
            public ICacheImplementation<TCache> InCloud { get; }
            private TCache CachedData;
            public TCache GetCachedData()
                => this.CachedData;
            public Instancer(ICacheKey<TCache> key, string keyString, ICacheImplementation<TCache> inCloud)
            {
                this.Key = key;
                this.KeyString = keyString;
                this.InCloud = inCloud;
                this.CloudIsActive = inCloud != null;
            }
            public async Task Execute(object state)
            {
                PromisedState promisedState = (PromisedState)state;
                try
                {
                    this.CachedData = await Key.FetchAsync().NoContext();
                    if (this.CachedData != null)
                    {
                        if (CloudIsActive)
                            await InCloud.UpdateAsync(KeyString, this.CachedData, default).NoContext();
                        promisedState.Status = PromisedStatus.Executed;
                    }
                    else
                        promisedState.Status = PromisedStatus.Empty;
                }
                catch (Exception ex)
                {
                    promisedState.Exception = ex;
                    promisedState.Status = PromisedStatus.InException;
                }
            }
        }
        private enum PromisedStatus
        {
            Ready,
            InExecution,
            Executed,
            Empty,
            InException
        }
        private class PromisedState
        {
            public PromisedStatus Status { get; set; }
            public Exception Exception { get; set; }
            public bool IsCompleted() => this.Status == PromisedStatus.Executed || this.HasEmptyResponse() || this.HasThrownAnException();
            public bool HasThrownAnException() => this.Status == PromisedStatus.InException;
            public bool HasEmptyResponse() => this.Status == PromisedStatus.Empty;
            public bool IsStarted() => this.Status == PromisedStatus.InExecution;
        }

        private class PromisedCache
        {
            public Instancer Instance { get; }
            public TCache Cache => Instance.GetCachedData();
            private readonly WaitCallback Executor;
            public PromisedState PromisedState { get; } = new PromisedState();
            public PromisedCache(Instancer instance)
            {
                this.Instance = instance;
                this.Executor = state => instance.Execute(state).ToResult();
            }
            private static readonly object TrafficLight = new object();
            public PromisedState Run()
            {
                if (this.PromisedState.Status == PromisedStatus.Ready)
                {
                    lock (TrafficLight)
                        if (this.PromisedState.Status == PromisedStatus.Ready)
                        {
                            this.PromisedState.Status = PromisedStatus.InExecution;
                            ThreadPool.UnsafeQueueUserWorkItem(this.Executor, this.PromisedState);
                        }
                }
                return this.PromisedState;
            }
        }
    }
}