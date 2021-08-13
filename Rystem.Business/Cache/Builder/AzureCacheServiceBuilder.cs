using Rystem.Azure.Integration.Cache;
using Rystem.Azure.Integration.Storage;

namespace Rystem.Business
{
    public class AzureCacheServiceBuilder : ServiceBuilder<RystemCacheServiceProvider>
    {
        private readonly CacheConfiguration Configuration;
        public AzureCacheServiceBuilder(Installation installation, CacheConfiguration configuration, ServiceProvider<RystemCacheServiceProvider> rystemServiceProvider) : base(installation, rystemServiceProvider) 
            => Configuration = configuration;
        public RystemCacheServiceProvider WithRedisCache(RedisCacheConfiguration configuration = default, string serviceKey = default)
            => (RystemCacheServiceProvider)WithIntegration(ServiceProviderType.AzureRedisCache, configuration, serviceKey, Configuration);
        public RystemCacheServiceProvider WithTableStorage(TableStorageConfiguration configuration = default, string serviceKey = default)
            => (RystemCacheServiceProvider)WithIntegration(ServiceProviderType.AzureTableStorage, configuration, serviceKey, Configuration);
        public RystemCacheServiceProvider WithBlobStorage(BlobStorageConfiguration configuration = default, string serviceKey = default)
            => (RystemCacheServiceProvider)WithIntegration(ServiceProviderType.AzureBlockBlobStorage, configuration, serviceKey, Configuration);
    }
}