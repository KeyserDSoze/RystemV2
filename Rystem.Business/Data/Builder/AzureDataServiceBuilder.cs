using Rystem.Azure.Integration.Storage;

namespace Rystem.Business
{
    public class AzureDataServiceBuilder : ServiceBuilder<RystemDataServiceProvider>
    {
        public AzureDataServiceBuilder(Installation installation, ServiceProvider<RystemDataServiceProvider> rystemServiceProvider) : base(installation, rystemServiceProvider)
        {
        }
        public RystemDataServiceProvider WithBlockBlob(BlobStorageConfiguration configuration = default, string serviceKey = default)
            => (RystemDataServiceProvider)WithIntegration(ServiceProviderType.AzureBlockBlobStorage, configuration, serviceKey);
        public RystemDataServiceProvider WithAppendBlob(BlobStorageConfiguration configuration = default, string serviceKey = default)
            => (RystemDataServiceProvider)WithIntegration(ServiceProviderType.AzureAppendBlobStorage, configuration, serviceKey);
    }
}