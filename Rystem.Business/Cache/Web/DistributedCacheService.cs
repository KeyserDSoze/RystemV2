using System;
using System.Threading.Tasks;

namespace Rystem.Cache
{
    public class DistributedCacheService : ICacheService
    {
        public Task<bool> ExistsAsync(string key)
            => new HttpResponseCacheKey { Key = key }.IsPresentAsync();

        public Task<HttpResponseCache> InstanceAsync(string key)
            => new HttpResponseCacheKey { Key = key }.InstanceAsync();

        public Task UpdateAsync(string key, HttpResponseCache entity, TimeSpan expireAfter)
            => new HttpResponseCacheKey { Key = key }.RestoreAsync(entity, expireAfter);
    }
}
