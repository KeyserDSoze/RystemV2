using Microsoft.Extensions.DependencyInjection;
using Rystem.Business;

namespace Rystem.Cache
{
    public static class CacheBuilderExtensions
    {
        public static RystemCacheServiceProvider<HttpResponseCacheKey, HttpResponseCache> AddAzureCache(this CacheBuilder cacheBuilder)
        {
            cacheBuilder.Services.AddSingleton<ICacheService, DistributedCacheService>();
            return new RystemCacheServiceProvider<HttpResponseCacheKey, HttpResponseCache>(cacheBuilder.Services);
        }
    }
}
