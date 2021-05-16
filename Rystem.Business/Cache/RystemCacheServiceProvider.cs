using System;
using System.Collections.Generic;

namespace Rystem.Business
{
    public sealed record InMemoryCacheConfiguration(TimeSpan ExpiringDefault) : Configuration(string.Empty);
    public sealed class RystemCacheServiceProvider : ServiceProvider<RystemCacheServiceProvider>
    {
        private RystemCacheServiceProvider() { }
        public static AzureCacheServiceBuilder WithAzure()
          => new RystemCacheServiceProvider().AndWithAzure();
        public AzureCacheServiceBuilder AndWithAzure()
          => new(Installation.Inst00, this);
        public static RystemCacheServiceProvider WithMemory(InMemoryCacheConfiguration configuration)
          => new RystemCacheServiceProvider().AndMemory(configuration);
        public RystemCacheServiceProvider AndMemory(InMemoryCacheConfiguration configuration)
        {
            Services.Add(Installation.Default, new ProvidedService(ServiceProviderType.InMemory, configuration, string.Empty));
            return this;
        }
    }
}