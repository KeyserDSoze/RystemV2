using Microsoft.Extensions.DependencyInjection;
using Rystem.Azure;

namespace Rystem
{
    public static class RystemBusiness
    {
        public static AzureBuilder WithAzure()
            => new ServiceCollection().AddAzureService();
        public static void FinalizeWithoutDependencyInjection(this IServiceCollection services)
        {
            services.AddRystem();
            DependencyInjectionExtensions.UseRystem(default);
        }
    }
}