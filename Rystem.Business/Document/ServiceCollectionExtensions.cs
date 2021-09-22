using Microsoft.Extensions.DependencyInjection;

namespace Rystem.Business
{
    public static partial class ServiceCollectionExtensions
    {
        public static RystemDocumentServiceProvider<TEntity> UseDocumentOn<TEntity>(this IServiceCollection services)
            where TEntity : new()
            => new(services);
    }
}