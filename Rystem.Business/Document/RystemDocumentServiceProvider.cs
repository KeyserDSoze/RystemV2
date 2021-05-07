using System;
using System.Collections.Generic;

namespace Rystem.Business
{
    public class RystemDocumentServiceProvider : RystemServiceProvider<RystemDocumentServiceProvider>
    {
        private RystemDocumentServiceProvider() { }
        public static AzureDocumentServiceBuilder WithAzure(Installation installation = Installation.Default)
          => new RystemDocumentServiceProvider().AndWithAzure(installation);
        public AzureDocumentServiceBuilder AndWithAzure(Installation installation = Installation.Default)
          => new(installation, this);
    }
}