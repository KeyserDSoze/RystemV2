using Rystem.Azure.Integration.Storage;
using Rystem.Azure.Integration.Cosmos;

namespace Rystem.Business
{
    public class AzureDocumentServiceBuilder : ServiceBuilder<RystemDocumentServiceProvider>
    {
        public AzureDocumentServiceBuilder(Installation installation, ServiceProvider<RystemDocumentServiceProvider> rystemServiceProvider) : base(installation, rystemServiceProvider)
        {
        }

        public RystemDocumentServiceProvider WithTableStorage(TableStorageConfiguration configuration = default, string serviceKey = default)
            => (RystemDocumentServiceProvider)WithIntegration(ServiceProviderType.AzureTableStorage, configuration, serviceKey);
        public RystemDocumentServiceProvider WithBlobStorage(BlobStorageConfiguration configuration = default, string serviceKey = default)
            => (RystemDocumentServiceProvider)WithIntegration(ServiceProviderType.AzureBlockBlobStorage, configuration, serviceKey);
        public RystemDocumentServiceProvider WithCosmosNoSql(CosmosConfiguration configuration = default, string serviceKey = default)
            => (RystemDocumentServiceProvider)WithIntegration(ServiceProviderType.AzureCosmosNoSql, configuration, serviceKey);
    }
}