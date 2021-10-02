using Rystem.Azure.Integration.Cache;
using Rystem.Azure.Integration.Storage;

namespace Rystem.Business
{
    public sealed class AzureCacheServiceBuilder<TCacheKey, TCache> : ServiceBuilder
           where TCacheKey : ICacheKey<TCache>
    {
        private readonly CacheConfiguration Configuration;
        public AzureCacheServiceBuilder(Installation installation, CacheConfiguration configuration, ServiceProvider rystemServiceProvider) : base(installation, rystemServiceProvider)
            => Configuration = configuration;
        public RystemCacheServiceProvider<TCacheKey, TCache> WithRedisCache(RedisCacheConfiguration configuration = default, string serviceKey = default)
            => (RystemCacheServiceProvider<TCacheKey, TCache>)WithIntegration<TCacheKey, RedisCacheConfiguration>(ServiceProviderType.AzureRedisCache, configuration, serviceKey, Configuration);
        public RystemCacheServiceProvider<TCacheKey, TCache> WithTableStorage(TableStorageConfiguration configuration = default, string serviceKey = default)
            => (RystemCacheServiceProvider<TCacheKey, TCache>)WithIntegration<TCacheKey, TableStorageConfiguration>(ServiceProviderType.AzureTableStorage, configuration, serviceKey, Configuration);
        public RystemCacheServiceProvider<TCacheKey, TCache> WithBlobStorage(BlobStorageConfiguration configuration = default, string serviceKey = default)
            => (RystemCacheServiceProvider<TCacheKey, TCache>)WithIntegration<TCacheKey, BlobStorageConfiguration>(ServiceProviderType.AzureBlockBlobStorage, configuration, serviceKey, Configuration);
    }
}