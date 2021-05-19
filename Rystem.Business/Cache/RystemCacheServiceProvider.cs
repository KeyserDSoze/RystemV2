using System;
using System.Collections.Generic;

namespace Rystem.Business
{
    public sealed record InMemoryCacheConfiguration(TimeSpan ExpiringDefault) : Configuration(string.Empty);
    public sealed class RystemCacheServiceProvider : ServiceProvider<RystemCacheServiceProvider>
    {
        private RystemCacheServiceProvider() { }
        public static AzureCacheServiceBuilder WithAzure(Installation installation = Installation.Default)
          => new RystemCacheServiceProvider().AndWithAzure(installation);
        public AzureCacheServiceBuilder AndWithAzure(Installation installation = Installation.Default)
          => new(installation, this);
        public static RystemCacheServiceProvider WithMemory(InMemoryCacheConfiguration configuration)
          => new RystemCacheServiceProvider().AndMemory(configuration);
        public RystemCacheServiceProvider AndMemory(InMemoryCacheConfiguration configuration)
        {
            Services.Add(Installation.Inst50, new ProvidedService(ServiceProviderType.InMemory, configuration, string.Empty));
            return this;
        }
    }
}