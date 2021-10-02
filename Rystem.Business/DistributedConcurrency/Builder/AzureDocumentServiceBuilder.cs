using Rystem.Azure.Integration.Cache;
using Rystem.Azure.Integration.Storage;
using Rystem.Business;

namespace Rystem.Concurrency
{
    public sealed class AzureDistributedServiceBuilder<T> : ServiceBuilder
        where T : IDistributedConcurrencyKey
    {
        public AzureDistributedServiceBuilder(Installation installation, ServiceProvider rystemServiceProvider) : base(installation, rystemServiceProvider)
        {
        }
        public RystemDistributedServiceProvider<T> WithBlobStorage(BlobStorageConfiguration configuration = default, string serviceKey = default)
            => (RystemDistributedServiceProvider<T>)WithIntegration<T, BlobStorageConfiguration>(ServiceProviderType.AzureBlockBlobStorage, configuration, serviceKey);
        public RystemDistributedServiceProvider<T> WithRedisCache(RedisCacheConfiguration configuration = default, string serviceKey = default)
            => (RystemDistributedServiceProvider<T>)WithIntegration<T, RedisCacheConfiguration>(ServiceProviderType.AzureRedisCache, configuration, serviceKey);
    }
}