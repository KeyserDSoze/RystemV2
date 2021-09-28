using Microsoft.Extensions.DependencyInjection;

namespace Rystem
{
    public static class ServiceLocatorExtensions
    {
        public static void FinalizeWithoutDependencyInjection(this IServiceCollection services)
        {
            services.AddRystem();
            DependencyInjectionExtensions.UseRystem(default);
        }
    }
}