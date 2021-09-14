using Microsoft.Extensions.DependencyInjection;

namespace Rystem.Cache
{
    public class CacheBuilder
    {
        private readonly IServiceCollection Services;
        public CacheBuilder(IServiceCollection services)
            => Services = services;
        public IServiceCollection Build() 
            => Services;
    }
}