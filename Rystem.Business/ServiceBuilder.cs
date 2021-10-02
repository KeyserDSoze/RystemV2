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
        private protected ServiceProvider WithIntegration<TName, TConfiguration>(ServiceProviderType serviceProviderType, TConfiguration configuration, string serviceKey, dynamic options = default)
            where TConfiguration : Configuration, new()
        {
            if (configuration == default)
                configuration = new TConfiguration() { Name = typeof(TName).Name };
            else if (configuration.Name == default)
                configuration = configuration with { Name = typeof(TName).Name };
            RystemServiceProvider.Services.Add(Installation,
                new ProvidedService(serviceProviderType, configuration, serviceKey ?? string.Empty, options));
            return RystemServiceProvider;
        }
    }
}