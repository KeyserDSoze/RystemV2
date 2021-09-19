using Microsoft.Extensions.DependencyInjection;

namespace Rystem.Business
{
    public static partial class IServiceCollectionExtensions
    {
        public static RystemDataServiceProvider<TEntity> UseDataOn<TEntity>(this IServiceCollection services)
            => new(services);
    }
}