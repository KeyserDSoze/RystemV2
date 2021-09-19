using Rystem.Business;
using System;
using System.Threading.Tasks;

namespace Rystem.Cache
{
    public class HttpResponseCacheKey : ICacheKey<HttpResponseCache>
    {
        public string Key { get; set; }
        internal static Func<RystemCacheServiceProvider<HttpResponseCacheKey, HttpResponseCache>> CacheServiceProvider;
        public Task<HttpResponseCache> FetchAsync() 
            => default;
    }
}
