using Microsoft.Extensions.DependencyInjection;

namespace Rystem.Business
{
    public static partial class IServiceCollectionExtensions
    {
        public static RystemQueueServiceProvider<TEntity> UseQueueOn<TEntity>(this IServiceCollection services)
            => new(services);
    }
}