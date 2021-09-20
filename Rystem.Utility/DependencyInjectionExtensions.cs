using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Rystem
{
    public static class DependencyInjectionExtensions
    {
        private static event Action AfterRystemIsFullyAdded;
        public static IHost WithRystem(this IHost host)
        {
            ServiceLocator.Services = host.Services;
            AfterRystemIsFullyAdded?.Invoke();
            return host;
        }
        public static IServiceCollection AddRystemFullyAddedCallback(this IServiceCollection services, Action action)
        {
            AfterRystemIsFullyAdded += action;
            return services;
        }
    }
}
