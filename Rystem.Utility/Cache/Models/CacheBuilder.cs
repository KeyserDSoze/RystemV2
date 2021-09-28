using Microsoft.Extensions.DependencyInjection;

namespace Rystem.Cache
{
    public sealed class CacheBuilder
    {
        public IServiceCollection Services { get; }
        public CacheBuilder(IServiceCollection services)
            => Services = services;
        public IServiceCollection Build() 
            => Services;
    }
}