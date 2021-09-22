using Microsoft.Extensions.DependencyInjection;
using Rystem.Background;

namespace Rystem.Business
{
    public static partial class ServiceCollectionExtensions
    {
        public static RystemAggregationServiceProvider<TEntity> UseAggregationOn<TEntity>(this IServiceCollection services)
            => new(services);
    }
}