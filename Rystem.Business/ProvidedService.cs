using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Rystem.Business
{
    public record Options<TManager>(Dictionary<Installation, ProvidedService> Services);
    public record ProvidedService(ServiceProviderType Type, dynamic Configurations, string ServiceKey, dynamic Options);
    public abstract class ServiceProvider
    {
        private protected readonly IServiceCollection ServiceCollection;
        internal ServiceProvider(IServiceCollection services) 
            => ServiceCollection = services;
        internal Dictionary<Installation, ProvidedService> Services { get; } = new();
    }
}