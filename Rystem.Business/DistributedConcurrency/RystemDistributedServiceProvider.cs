using Rystem.Business;
using System;
using System.Collections.Generic;

namespace Rystem.Concurrency
{
    public class RystemDistributedServiceProvider : ServiceProvider<RystemDistributedServiceProvider>
    {
        private RystemDistributedServiceProvider() { }
        public static AzureDistributedServiceBuilder WithAzure(Installation installation = Installation.Default)
          => new RystemDistributedServiceProvider().AndWithAzure(installation);
        public AzureDistributedServiceBuilder AndWithAzure(Installation installation = Installation.Default)
          => new(installation, this);
    }
}