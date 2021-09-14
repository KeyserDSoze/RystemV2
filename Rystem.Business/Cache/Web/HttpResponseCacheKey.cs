using Rystem.Business;
using System;
using System.Threading.Tasks;

namespace Rystem.Cache
{
    public class HttpResponseCacheKey : ICacheKey<HttpResponseCache>
    {
        public string Key { get; set; }
        internal static Func<RystemCacheServiceProvider> CacheServiceProvider;
        public RystemCacheServiceProvider ConfigureCache() 
            => CacheServiceProvider.Invoke();
        public Task<HttpResponseCache> FetchAsync() 
            => default;
    }
}
