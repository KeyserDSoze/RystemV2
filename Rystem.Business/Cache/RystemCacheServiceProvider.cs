using System;
using System.Collections.Generic;

namespace Rystem.Business
{
    public class RystemCacheServiceProvider : ServiceProvider<RystemCacheServiceProvider>
    {
        private RystemCacheServiceProvider() { }
        public static AzureCacheServiceBuilder WithAzure()
          => new RystemCacheServiceProvider().AndWithAzure();
        public AzureCacheServiceBuilder AndWithAzure()
          => new(Installation.Inst00, this);
        public static RystemCacheServiceProvider WithMemory()
          => new RystemCacheServiceProvider().AndWithAzure();
        public RystemCacheServiceProvider AndMemory()
          => new(Installation.Default, this);
    }
}