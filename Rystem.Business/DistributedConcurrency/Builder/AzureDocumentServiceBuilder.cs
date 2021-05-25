using Rystem.Azure.Integration.Cache;
using Rystem.Azure.Integration.Storage;
using Rystem.Business;

namespace Rystem.Concurrency
{
    public class AzureDistributedServiceBuilder : ServiceBuilder<RystemDistributedServiceProvider>
    {
        public AzureDistributedServiceBuilder(Installation installation, ServiceProvider<RystemDistributedServiceProvider> rystemServiceProvider) : base(installation, rystemServiceProvider)
        {
        }
        public RystemDistributedServiceProvider WithBlobStorage(BlobStorageConfiguration configuration = default, string serviceKey = default)
            => (RystemDistributedServiceProvider)WithIntegration(ServiceProviderType.AzureBlockBlobStorage, configuration, serviceKey);
        public RystemDistributedServiceProvider WithRedisCache(RedisCacheConfiguration configuration = default, string serviceKey = default)
            => (RystemDistributedServiceProvider)WithIntegration(ServiceProviderType.AzureRedisCache, configuration, serviceKey);

    }
}