using Microsoft.Extensions.DependencyInjection;
using Rystem.Azure.Integration.Cache;
using Rystem.Azure.Integration.Message;
using Rystem.Azure.Integration.Secrets;
using Rystem.Azure.Integration.Storage;
using Rystem.Azure.Integration.Cosmos;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Rystem.Azure
{
    public class AzureBuilder
    {
        internal static readonly AzureManager Manager = new();
        private readonly IServiceCollection Services;
        internal AzureBuilder(IServiceCollection services)
            => Services = services;
        public AzureBuilder AddStorage(StorageAccount account, string key = "")
            => Add(account, AzureServiceProviderType.Storage, key);
        public AzureBuilder AddCosmos(CosmosAccount account, string key = "")
            => Add(account, AzureServiceProviderType.Cosmos, key);
        public AzureBuilder AddEventHub(EventHubAccount account, string key = "")
            => Add(account, AzureServiceProviderType.EventHub, key);
        public AzureBuilder AddServiceBus(ServiceBusAccount account, string key = "")
            => Add(account, AzureServiceProviderType.ServiceBus, key);
        public AzureBuilder AddRedisCache(RedisCacheAccount account, string key = "")
            => Add(account, AzureServiceProviderType.RedisCache, key);
        public AzureBuilder AddKeyVault(KeyVaultAccount account, string key = "")
            => Add(account, AzureServiceProviderType.KeyVault, key);
        private AzureBuilder Add<T>(T account, AzureServiceProviderType service, string key)
            where T : class
        {
            Manager.AddAccount(account, service, key);
            Services.TryAddSingleton(Manager);
            return this;
        }
        public IServiceCollection EndConfiguration()
            => Services;
    }
}