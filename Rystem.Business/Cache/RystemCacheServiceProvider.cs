using System;
using System.Collections.Generic;

namespace Rystem.Business
{
    public sealed record CacheConfiguration(TimeSpan ExpiringDefault) : Configuration(string.Empty);
    public sealed class RystemCacheServiceProvider : ServiceProvider<RystemCacheServiceProvider>
    {
        private RystemCacheServiceProvider() { }
        public static AzureCacheServiceBuilder WithAzure(CacheConfiguration configuration = default, Installation installation = Installation.Default)
          => new RystemCacheServiceProvider().AndWithAzure(configuration, installation);
        public AzureCacheServiceBuilder AndWithAzure(CacheConfiguration configuration = default, Installation installation = Installation.Default)
          => new(installation, configuration, this);
        public static RystemCacheServiceProvider WithMemory(CacheConfiguration configuration)
          => new RystemCacheServiceProvider().AndMemory(configuration);
        public RystemCacheServiceProvider AndMemory(CacheConfiguration configuration)
        {
            Services.Add(Installation.Memory, new ProvidedService(ServiceProviderType.InMemory, default, string.Empty, configuration));
            return this;
        }
    }
}