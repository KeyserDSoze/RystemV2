using Microsoft.Extensions.DependencyInjection;

namespace Rystem.Business
{
    public static partial class ServiceCollectionExtensions
    {
        public static RystemDataServiceProvider<TEntity> UseDataOn<TEntity>(this IServiceCollection services)
            => new(services);
    }
}