using System;
using System.Threading.Tasks;

namespace Rystem.Cache
{
    public sealed class MemoryCacheService : ICacheService
    {
        public Task<bool> ExistsAsync(string key) 
            => Task.FromResult(new Key(key).Exists<HttpResponseCache>());

        public Task<HttpResponseCache> InstanceAsync(string key) 
            => Task.FromResult(new Key(key).Instance<HttpResponseCache>());

        public Task UpdateAsync(string key, HttpResponseCache entity, TimeSpan expireAfter)
        {
            new Key(key).Update(entity, expireAfter);
            return Task.CompletedTask;
        }
    }
}