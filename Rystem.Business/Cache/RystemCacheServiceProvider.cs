using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Rystem.Business
{
    public sealed record CacheConfiguration(TimeSpan ExpiringDefault) : Configuration(string.Empty);
    public sealed class RystemCacheServiceProvider<TCacheKey, TCache> : ServiceProvider
        where TCacheKey : ICacheKey<TCache>
    {
        internal RystemCacheServiceProvider(IServiceCollection services) : base(services) { }
        public AzureCacheServiceBuilder<TCacheKey, TCache> WithAzure(CacheConfiguration configuration = default, Installation installation = Installation.Default)
          => AndWithAzure(configuration, installation);
        public AzureCacheServiceBuilder<TCacheKey, TCache> AndWithAzure(CacheConfiguration configuration = default, Installation installation = Installation.Default)
          => new(installation, configuration, this);
        public RystemCacheServiceProvider<TCacheKey, TCache> WithMemory(CacheConfiguration configuration = default)
          => AndMemory(configuration);
        public RystemCacheServiceProvider<TCacheKey, TCache> AndMemory(CacheConfiguration configuration = default)
        {
            Services.Add(Installation.Memory, new ProvidedService(ServiceProviderType.InMemory, default, string.Empty, configuration ?? new CacheConfiguration(default)));
            return this;
        }
        public IServiceCollection Configure()
        {
            bool memoryIsActive = Services.ContainsKey(Installation.Memory);
            var cacheConfigurations = Services.ToDictionary(x => x.Key, x => x.Value);
            string name = typeof(TCacheKey).Name;
            var manager = new CacheManager<TCacheKey, TCache>(memoryIsActive, cacheConfigurations, name);
            ServiceCollection.AddSingleton(manager);
            return ServiceCollection;
        }
    }
}