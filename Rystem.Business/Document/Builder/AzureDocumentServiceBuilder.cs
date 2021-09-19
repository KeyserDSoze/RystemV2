using Rystem.Azure.Integration.Storage;
using Rystem.Azure.Integration.Cosmos;

namespace Rystem.Business
{
    public class AzureDocumentServiceBuilder<T> : ServiceBuilder
        where T : new()
    {
        public AzureDocumentServiceBuilder(Installation installation, ServiceProvider rystemServiceProvider) : base(installation, rystemServiceProvider)
        {
        }

        public RystemDocumentServicePrimaryKey<T> WithTableStorage(TableStorageConfiguration configuration = default, string serviceKey = default)
        {
            var options = new RystemDocumentServiceProviderOptions();
            return new RystemDocumentServicePrimaryKey<T>(WithIntegration(ServiceProviderType.AzureTableStorage, configuration, serviceKey, options), options);
        }

        public RystemDocumentServicePrimaryKey<T> WithBlobStorage(BlobStorageConfiguration configuration = default, string serviceKey = default)
        {
            var options = new RystemDocumentServiceProviderOptions();
            return new RystemDocumentServicePrimaryKey<T>(WithIntegration(ServiceProviderType.AzureBlockBlobStorage, configuration, serviceKey, options), options);
        }

        public RystemDocumentServicePrimaryKey<T> WithCosmosNoSql(CosmosConfiguration configuration = default, string serviceKey = default)
        {
            var options = new RystemDocumentServiceProviderOptions();
            return new RystemDocumentServicePrimaryKey<T>(WithIntegration(ServiceProviderType.AzureCosmosNoSql, configuration, serviceKey, options), options);
        }
    }
}