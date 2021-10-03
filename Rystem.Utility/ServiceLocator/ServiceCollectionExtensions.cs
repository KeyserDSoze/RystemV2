using Microsoft.Extensions.DependencyInjection;

namespace Rystem
{
    public static partial class ServiceCollectionExtensions
    {
        public static void FinalizeWithoutDependencyInjection(this IServiceCollection services)
        {
            services.AddRystem();
            DependencyInjectionExtensions.UseRystem(default);
        }
    }
}