using Microsoft.Extensions.DependencyInjection;
using Rystem.Business;

namespace Rystem
{
    public static partial class ServiceCollectionExtensions
    {
        public static RystemCacheServiceProvider<TKey, TInstance> UseCacheWithKey<TKey, TInstance>(this IServiceCollection services)
            where TKey : ICacheKey<TInstance> 
            => new(services);
    }
}