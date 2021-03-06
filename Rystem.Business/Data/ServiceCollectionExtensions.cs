using Microsoft.Extensions.DependencyInjection;
using Rystem.Business;

namespace Rystem
{
    public static partial class ServiceCollectionExtensions
    {
        public static RystemDataServiceProvider<TEntity> UseDataOn<TEntity>(this IServiceCollection services)
            => new(services);
    }
}