using Rystem.Azure.Integration.Storage;

namespace Rystem.Business
{
    public sealed class AzureDataServiceBuilder<T> : ServiceBuilder
    {
        public AzureDataServiceBuilder(Installation installation, ServiceProvider rystemServiceProvider) : base(installation, rystemServiceProvider)
        {
        }
        public RystemDataServiceName<T> WithBlockBlob(BlobStorageConfiguration configuration = default, string serviceKey = default)
        {
            var option = new RystemDataServiceProviderOptions();
            return new RystemDataServiceName<T>(WithIntegration<T, BlobStorageConfiguration>(ServiceProviderType.AzureBlockBlobStorage, configuration, serviceKey, option), option);
        }
        public RystemDataServiceName<T> WithAppendBlob(BlobStorageConfiguration configuration = default, string serviceKey = default)
        {
            var option = new RystemDataServiceProviderOptions();
            return new RystemDataServiceName<T>(WithIntegration<T, BlobStorageConfiguration>(ServiceProviderType.AzureAppendBlobStorage, configuration, serviceKey, option), option);
        }
    }
}