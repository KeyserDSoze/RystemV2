using Rystem.Azure.Integration.Cache;
using Rystem.Azure.Integration.Message;
using Rystem.Azure.Integration.Secrets;
using Rystem.Azure.Integration.Storage;
using Rystem.Azure.Integration.Cosmos;
using System.Collections.Generic;

namespace Rystem.Azure
{
    public sealed class AzureManager
    {
        private readonly Dictionary<string, dynamic> Accounts = new();
        internal void AddAccount<T>(T entity, AzureServiceProviderType service, string key) 
            => Accounts.TryAdd($"{service}-{key}", entity);
        public KeyVaultIntegration KeyVault(string key = "")
         => new(Accounts[$"{AzureServiceProviderType.KeyVault}-{key}"]);
        public RedisCacheIntegration RedisCache(string key = "")
           => new(Accounts[$"{AzureServiceProviderType.RedisCache}-{key}"]);
        public BlobStorageIntegration BlobStorage(BlobStorageConfiguration configuration, string key = "")
           => new(configuration, Accounts[$"{AzureServiceProviderType.Storage}-{key}"]);
        public TableStorageIntegration TableStorage(TableStorageConfiguration configuration, string key = "")
           => new(configuration, Accounts[$"{AzureServiceProviderType.Storage}-{key}"]);
        public QueueStorageIntegration QueueStorage(QueueStorageConfiguration configuration, string key = "")
           => new(configuration, Accounts[$"{AzureServiceProviderType.Storage}-{key}"]);
        public EventHubIntegration EventHub(EventHubConfiguration configuration, string key = "")
           => new(configuration, Accounts[$"{AzureServiceProviderType.EventHub}-{key}"]);
        public ServiceBusIntegration ServiceBus(ServiceBusConfiguration configuration, string key = "")
           => new(configuration, Accounts[$"{AzureServiceProviderType.ServiceBus}-{key}"]);
        public CosmosNoSqlIntegration CosmosNoSql(CosmosConfiguration configuration, string key = "")
           => new(configuration, Accounts[$"{AzureServiceProviderType.Cosmos}-{key}"]);
    }
}