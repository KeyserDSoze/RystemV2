using Microsoft.Extensions.DependencyInjection;
using Rystem.Concurrency;

namespace Rystem
{
    public static partial class ServiceCollectionExtensions
    {
        public static RystemDistributedServiceProvider<TKey> UseDistributedKey<TKey>(this IServiceCollection services)
            where TKey : IDistributedConcurrencyKey
            => new(services);
    }
}