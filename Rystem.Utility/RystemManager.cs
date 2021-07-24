using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Rystem
{
    public class RystemManager
    {
        public static T GetService<T>() => (T)(Services ??= ServiceCollection?.BuildServiceProvider()).GetService(typeof(T));
        internal static IServiceProvider Services { get; set; }
        public static IServiceCollection ServiceCollection { get; set; }
    }
    public static class RystemManagerExtesions
    {
        public static IHost WithRystem(this IHost host)
        {
            RystemManager.Services = host.Services;
            return host;
        }
        public static IServiceCollection WithRystem(this IServiceCollection services)
        {
            RystemManager.ServiceCollection = services;
            return services;
        }
    }
}
