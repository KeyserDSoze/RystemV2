using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Rystem
{
    public class RystemManager
    {
        public static T GetService<T>()
        {
            if (Count != ServiceCollection.Count)
            {
                Services = ServiceCollection.BuildServiceProvider();
                Count = ServiceCollection.Count;
            }
            return (T)Services.GetService(typeof(T));
        }
        private static int Count = 0;
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
