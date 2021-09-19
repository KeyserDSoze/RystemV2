using Microsoft.Extensions.DependencyInjection;

namespace Rystem.Azure
{
    public static class IServiceCollectionExtensions
    {
        public static AzureBuilder AddAzureService(this IServiceCollection services)
            => new(services ?? new ServiceCollection());
    }
}