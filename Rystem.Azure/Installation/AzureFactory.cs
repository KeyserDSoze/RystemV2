using Rystem.Azure.Integration.Cache;
using Rystem.Azure.Integration.Message;
using Rystem.Azure.Integration.Secrets;
using Rystem.Azure.Integration.Storage;
using Rystem.Azure.Integration.Cosmos;
using System;
using System.Collections.Generic;

namespace Rystem.Azure
{
    public sealed class AzureFactory
    {
        public AzureManager Manager { get; }
        public AzureFactory(AzureManager manager) => Manager = manager;
        private readonly Dictionary<string, dynamic> Integrations = new();
        public RedisCacheIntegration RedisCache(string key = "")
            => Get<RedisCacheIntegration>("RedisCache", key, () => new(Manager.RedisCaches[key]));
        public EventHubIntegration EventHub(EventHubConfiguration eventHubConfiguration, string key = "")
            => Get<EventHubIntegration>("EventHub", $"{key}-{eventHubConfiguration.Name}", () => new(eventHubConfiguration, Manager.EventHubs[key]));
        public ServiceBusIntegration ServiceBus(ServiceBusConfiguration serviceBusConfiguration, string key = "")
            => Get<ServiceBusIntegration>("ServiceBus", $"{key}-{serviceBusConfiguration.Name}", () => new(serviceBusConfiguration, Manager.ServiceBuses[key]));
        public KeyVaultIntegration KeyVault(string key = "")
            => Get<KeyVaultIntegration>("KeyVault", key, () => new(Manager.KeyVaults[key]));
        public QueueStorageIntegration QueueStorage(QueueStorageConfiguration queueStorageConfiguration, string key = "")
            => Get<QueueStorageIntegration>("QueueStorage", $"{key}-{queueStorageConfiguration.Name}", () => new(queueStorageConfiguration, Manager.Storages[key]));
        public BlobStorageIntegration BlobStorage(BlobStorageConfiguration blobStorageConfiguration, string key = "")
            => Get<BlobStorageIntegration>("BlobStorage", $"{key}-{blobStorageConfiguration.Name}", () => new(blobStorageConfiguration, Manager.Storages[key]));
        public TableStorageIntegration TableStorage(TableStorageConfiguration tableStorageConfiguration, string key = "")
            => Get<TableStorageIntegration>("TableStorage", $"{key}-{tableStorageConfiguration.Name}", () => new(tableStorageConfiguration, Manager.Storages[key]));
        public CosmosNoSqlIntegration CosmosNoSql(CosmosConfiguration cosmosNoSqlConfiguration, string key = "")
            => Get<CosmosNoSqlIntegration>("CosmosNoSql", $"{key}-{cosmosNoSqlConfiguration.Name}-{cosmosNoSqlConfiguration.ContainerProperties?.Id}", () => new(cosmosNoSqlConfiguration, Manager.Cosmos[key]));
        public Dictionary<string, StorageOptions> Storages { get; } = new();
        private readonly object TrafficLight = new();
        private T Get<T>(string baseKey, string key, Func<T> retrieveInstance)
        {
            string computedKey = $"{baseKey}-{key}";
            if (!Integrations.ContainsKey(computedKey))
                lock (TrafficLight)
                    if (!Integrations.ContainsKey(computedKey))
                        Integrations.Add(computedKey, retrieveInstance.Invoke());
            return Integrations[computedKey];
        }
    }
}