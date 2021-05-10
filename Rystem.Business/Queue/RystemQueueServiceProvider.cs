using System;
using System.Collections.Generic;

namespace Rystem.Business
{
    public class RystemQueueServiceProvider : ServiceProvider<RystemQueueServiceProvider>
    {
        private RystemQueueServiceProvider() { }
        public static AzureQueueServiceBuilder WithAzure(Installation installation = Installation.Default)
          => new RystemQueueServiceProvider().AndWithAzure(installation);
        public AzureQueueServiceBuilder AndWithAzure(Installation installation = Installation.Default)
          => new(installation, this);
    }
}