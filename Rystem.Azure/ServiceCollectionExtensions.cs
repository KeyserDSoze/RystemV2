using Microsoft.Extensions.DependencyInjection;
using Rystem.Azure;

namespace Rystem
{
    public static class ServiceCollectionExtensions
    {
        public static AzureBuilder AddAzureService(this IServiceCollection services)
            => new(services);
    }
}