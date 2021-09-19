using Microsoft.Extensions.DependencyInjection;
using Rystem.Concurrency;

namespace Rystem.Business
{
    public static partial class IServiceCollectionExtensions
    {
        public static RystemDistributedServiceProvider<TKey> UseDistributedKey<TKey>(this IServiceCollection services)
            where TKey : IDistributedConcurrencyKey
            => new(services);
    }
}