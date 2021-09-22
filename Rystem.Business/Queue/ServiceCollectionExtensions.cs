using Microsoft.Extensions.DependencyInjection;

namespace Rystem.Business
{
    public static partial class ServiceCollectionExtensions
    {
        public static RystemQueueServiceProvider<TEntity> UseQueueOn<TEntity>(this IServiceCollection services)
            => new(services);
    }
}