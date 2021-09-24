using Microsoft.Extensions.DependencyInjection;
using System;

namespace Rystem
{
    public class ServiceLocator
    {
        public static T GetService<T>() => (T)Providers.GetService(typeof(T));
        public static object GetService(Type type) => Providers.GetService(type);
        internal static IServiceCollection Services { get; set; }
        private static IServiceProvider providers;
        internal static IServiceProvider Providers => providers ??= Services.BuildServiceProvider();
    }
}