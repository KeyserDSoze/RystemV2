using Rystem.Reflection;

namespace Rystem.Business
{
    public abstract class ServiceBuilder
    {
        private protected readonly ServiceProvider RystemServiceProvider;
        private protected readonly Installation Installation;
        public ServiceBuilder(Installation installation, ServiceProvider rystemServiceProvider)
        {
            RystemServiceProvider = rystemServiceProvider;
            Installation = installation;
        }
        private protected ServiceProvider WithIntegration<TConfiguration>(ServiceProviderType serviceProviderType, TConfiguration configuration, string serviceKey, dynamic options = default)
            where TConfiguration : Configuration, new()
        {
            if (configuration == default)
                configuration = new TConfiguration() { Name = ReflectionHelper.NameOfCallingClass(2) };
            else if (configuration.Name == default)
                configuration = configuration with { Name = ReflectionHelper.NameOfCallingClass(2) };
            RystemServiceProvider.Services.Add(Installation,
                new ProvidedService(serviceProviderType, configuration, serviceKey ?? string.Empty, options));
            return RystemServiceProvider;
        }
    }
}