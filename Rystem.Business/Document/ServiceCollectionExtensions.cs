using Microsoft.Extensions.DependencyInjection;
using Rystem.Business;

namespace Rystem
{
    public static partial class ServiceCollectionExtensions
    {
        public static RystemDocumentServiceProvider<TEntity> UseDocumentOn<TEntity>(this IServiceCollection services)
            where TEntity : new()
            => new(services);
    }
}