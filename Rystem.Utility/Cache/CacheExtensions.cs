using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Builder;
using Rystem.Cache;

namespace Rystem
{
    public static class CacheExtensions
    {
        public static CacheBuilder AddCache(this IServiceCollection services, Action<CacheOptions> action)
        {
            CacheOptions options = new();
            action.Invoke(options);
            services.AddSingleton(options);
            services.AddSingleton<CacheMiddleware>();
            services.AddSingleton<ICacheService, MemoryCacheService>();
            return new CacheBuilder(services);
        }
        public static IApplicationBuilder UseCache(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<CacheMiddleware>();
            return applicationBuilder;
        }
    }
}