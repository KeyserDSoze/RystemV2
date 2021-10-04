using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Rystem
{
    public abstract class ServiceProvider
    {
        protected readonly IServiceCollection ServiceCollection;
        protected ServiceProvider(IServiceCollection services) 
            => ServiceCollection = services;
        public Dictionary<Installation, ProvidedService> Services { get; } = new();
    }
}