﻿using Microsoft.Extensions.DependencyInjection;

namespace Rystem.Business
{
    public static partial class IServiceCollectionExtensions
    {
        public static RystemCacheServiceProvider<TKey, TInstance> UseCacheWithKey<TKey, TInstance>(this IServiceCollection services)
            where TKey : ICacheKey<TInstance> 
            => new(services);
    }
}