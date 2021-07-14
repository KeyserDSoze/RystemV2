using Microsoft.Extensions.DependencyInjection;
using Rystem.Azure.Installation;
using Rystem.Reflection;

namespace Rystem.Business
{
    public abstract class ServiceBuilder<T>
        where T : ServiceProvider<T>
    {
        private protected readonly ServiceProvider<T> RystemServiceProvider;
        private protected readonly Installation Installation;
        public ServiceBuilder(Installation installation, ServiceProvider<T> rystemServiceProvider)
        {
            RystemServiceProvider = rystemServiceProvider;
            Installation = installation;
        }
        private protected ServiceProvider<T> WithIntegration<TConfiguration>(ServiceProviderType serviceProviderType, TConfiguration configuration, string serviceKey)
            where TConfiguration : Configuration, new()
        {
            if (configuration == default)
                configuration = new TConfiguration() { Name = ReflectionHelper.NameOfCallingClass(2) };
            else if (configuration.Name == default)
                configuration = configuration with { Name = ReflectionHelper.NameOfCallingClass(2) };
            RystemServiceProvider.Services.Add(Installation,
                new ProvidedService(serviceProviderType, configuration, serviceKey ?? string.Empty));
            return RystemServiceProvider;
        }
    }
}
