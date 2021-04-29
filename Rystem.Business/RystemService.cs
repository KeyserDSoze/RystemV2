using System;
using System.Collections.Generic;

namespace Rystem.Business
{
    public enum RystemServiceType
    {
        AzureKeyVault,
        AzureTableStorage,
        AzureBlobStorage,
        AzureQueueStorage,
        AzureEventHub,
        AzureServiceBus,
        AzureRedisCache
    }
    public class RystemService
    {
        public RystemServiceType Type { get; init; }
        public dynamic Service { get; init; }
    }
    public class RystemServices
    {
        public Type InstanceType { get; private set; }
        internal Dictionary<Installation, RystemService> Services { get; } = new();
        private RystemServices() { }
        public static AzureService WithAzure()
            => new(new RystemServices());
        internal RystemServices AddInstance(Type instanceType)
        {
            this.InstanceType = instanceType;
            return this;
        }
    }

    public class AzureService
    {
        private readonly RystemServices RystemService;
        public AzureService(RystemServices rystemService)
            => RystemService = rystemService;
        public AzureDocumentServiceBuilder UseDocument(Installation installation = Installation.Default)
            => new(installation, RystemService);
        public AzureDocumentServiceBuilder UseData(Installation installation = Installation.Default)
            => new(installation, RystemService);
        public AzureDocumentServiceBuilder UseQueue(Installation installation = Installation.Default)
            => new(installation, RystemService);
        public AzureDocumentServiceBuilder UseAggregation(Installation installation = Installation.Default)
            => new(installation, RystemService);
        public AzureDocumentServiceBuilder UseCache(Installation installation = Installation.Default)
            => new(installation, RystemService);
    }
    public interface IDocument
    {
        RystemServices Configure();
        public RystemServices Build()
        {
            return Configure().AddInstance(this.GetType());
        }
    }
   
}