using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Rystem
{
    public static class ServiceLocator
    {
        public static T GetService<T>() => (T)Providers.GetService(typeof(T));
        public static object GetService(Type type) => Providers.GetService(type);
        public static bool HasService<T>() => Providers.GetService(typeof(T)) != default;
        public static bool HasService(Type type) => Providers.GetService(type) != default;
        internal static IServiceCollection Services { get; set; }
        private static IServiceProvider providers;
        internal static IServiceProvider Providers => providers ??= Services.BuildServiceProvider();
        internal static void ResetProviders()
        {
            providers = Services.BuildServiceProvider();
            if (ApplicationBuilder != default)
                ApplicationBuilder.ApplicationServices = providers;
        }
        public static IServiceCollection Create() => Services = new ServiceCollection();
        internal static IApplicationBuilder ApplicationBuilder { get; set; }
    }
}