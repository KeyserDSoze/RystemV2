using Rystem.Azure.Integration.Cache;
using Rystem.Text;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rystem.Business
{
    internal sealed class InRedisCache<T> : ICacheImplementation<T>
    {
        private readonly RedisCacheIntegration Integration;
        private readonly string Prefix;
        public InRedisCache(RedisCacheIntegration integration, string prefix)
        {
            Integration = integration;
            Prefix = prefix;
        }
        private string GetKeyWithPrefix(string key)
            => $"{Prefix}_{key}";
        public async Task<T> InstanceAsync(string key)
        {
            string json = await Integration.InstanceAsync(GetKeyWithPrefix(key)).NoContext();
            return json.FromJson<T>();
        }
        public async Task<bool> UpdateAsync(string key, T value, TimeSpan expiringTime)
            => await Integration.UpdateAsync(GetKeyWithPrefix(key), value.ToJson(), expiringTime);
        public async Task<CacheStatus<T>> ExistsAsync(string key)
            => await Integration.ExistsAsync(GetKeyWithPrefix(key)).NoContext() ? CacheStatus<T>.Ok() : CacheStatus<T>.NotOk();
        public async Task<bool> DeleteAsync(string key)
            => await Integration.DeleteAsync(GetKeyWithPrefix(key)).NoContext();
        public async Task<IEnumerable<string>> ListAsync()
        {
            var checkPrefix = $"{Prefix}_";
            List<string> keys = new();
            await foreach (var key in Integration.ListKeysAsync(default))
                if (key.StartsWith(checkPrefix))
                    keys.Add(key);
            return keys;
        }
        public Task<bool> WarmUpAsync()
            => Integration.WarmUpAsync();
    }
}