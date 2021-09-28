using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Rystem
{
    public static class DependencyInjectionExtensions
    {
        private static event Action AfterRystemIsFullyAdded;
        public static IApplicationBuilder UseRystem(this IApplicationBuilder applicationBuilder)
        {
            _ = ServiceLocator.Providers;
            AfterRystemIsFullyAdded?.Invoke();
            return applicationBuilder;
        }

        public static IServiceCollection AddRystemFullyAddedCallback(this IServiceCollection services, Action action)
        {
            AfterRystemIsFullyAdded += action;
            return services;
        }
        public static IServiceCollection AddRystem(this IServiceCollection services)
            => ServiceLocator.Services = services;
    }
}