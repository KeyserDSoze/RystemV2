using Microsoft.Extensions.DependencyInjection;
using Rystem.Business;
using System;

namespace Rystem.Cache
{
    public static class CacheBuilderExtensions
    {
        public static IServiceCollection AddAzureCache(this CacheBuilder cacheBuilder, Func<RystemCacheServiceProvider> cacheServiceProvider)
        {
            HttpResponseCacheKey.CacheServiceProvider = cacheServiceProvider;
            return cacheBuilder.Build()
                .AddSingleton<ICacheService, DistributedCacheService>();
        }
    }
}
