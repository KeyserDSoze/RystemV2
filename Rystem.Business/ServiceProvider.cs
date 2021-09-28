using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Rystem.Business
{
    public abstract class ServiceProvider
    {
        private protected readonly IServiceCollection ServiceCollection;
        internal ServiceProvider(IServiceCollection services) 
            => ServiceCollection = services;
        internal Dictionary<Installation, ProvidedService> Services { get; } = new();
    }
}