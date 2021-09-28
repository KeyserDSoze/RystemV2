using Microsoft.Extensions.DependencyInjection;
using Rystem.Business;

namespace Rystem
{
    public static partial class ServiceCollectionExtensions
    {
        public static RystemQueueServiceProvider<TEntity> UseQueueOn<TEntity>(this IServiceCollection services)
            => new(services);
    }
}