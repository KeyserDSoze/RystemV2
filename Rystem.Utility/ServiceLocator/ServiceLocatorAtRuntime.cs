using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Rystem
{
    public static class ServiceLocatorAtRuntime
    {
        public static IServiceCollection PrepareToAddNewService()
            => ServiceLocator.Services;
        public static void Rebuild()
        {
            Rebuild(ServiceLocator.GetService);
        }
        private static readonly object Semaphore = new();
        internal static void Rebuild(Func<Type, object> retrieveObject)
        {
            lock (Semaphore)
            {
                var services = new ServiceCollection();
                foreach (var service in ServiceLocator.Services)
                {
                    if (service.Lifetime == ServiceLifetime.Singleton)
                    {
                        var actualService = Try.Execute(() => retrieveObject(service.ImplementationType ?? service.ServiceType), 0).Result;
                        if (actualService != default)
                            services.AddSingleton(service.ServiceType, actualService);
                        else
                        {
                            if (service.ImplementationInstance != default)
                                services.AddSingleton(service.ServiceType, service.ImplementationInstance);
                            else
                                services.AddSingleton(service.ServiceType, service.ImplementationType ?? service.ServiceType);
                        }
                    }
                    else
                        services.Add(service);
                }
                ServiceLocator.Services = services;
                ServiceLocator.ResetProviders();
            }
        }
    }
}