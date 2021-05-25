using Rystem.Azure.Integration.Cache;
using Rystem.Azure.Integration.Storage;

namespace Rystem.Business
{
    public class AzureCacheServiceBuilder : ServiceBuilder<RystemCacheServiceProvider>
    {
        public AzureCacheServiceBuilder(Installation installation, ServiceProvider<RystemCacheServiceProvider> rystemServiceProvider) : base(installation, rystemServiceProvider)
        {
        }
        public RystemCacheServiceProvider WithRedisCache(RedisCacheConfiguration configuration = default, string serviceKey = default)
            => (RystemCacheServiceProvider)WithIntegration(ServiceProviderType.AzureRedisCache, configuration, serviceKey);
        public RystemCacheServiceProvider WithTableStorage(TableStorageConfiguration configuration = default, string serviceKey = default)
            => (RystemCacheServiceProvider)WithIntegration(ServiceProviderType.AzureTableStorage, configuration, serviceKey);
        public RystemCacheServiceProvider WithBlobStorage(BlobStorageConfiguration configuration = default, string serviceKey = default)
            => (RystemCacheServiceProvider)WithIntegration(ServiceProviderType.AzureBlockBlobStorage, configuration, serviceKey);
    }
}