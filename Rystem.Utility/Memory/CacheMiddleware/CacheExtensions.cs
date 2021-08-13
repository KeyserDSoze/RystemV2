using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Builder;

namespace Rystem.Memory
{
    public static class CacheExtensions
    {
        public static IServiceCollection AddCacheInMemory(this IServiceCollection services, Action<CacheOptions> action)
        {
            CacheOptions options = new();
            action.Invoke(options);
            services.AddSingleton(options);
            services.AddSingleton<CacheMiddleware>();
            return services;
        }
        public static IApplicationBuilder UseCacheInMemory(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<CacheMiddleware>();
            return applicationBuilder;
        }
    }
}