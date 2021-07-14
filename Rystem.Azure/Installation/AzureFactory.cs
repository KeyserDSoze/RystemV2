using Microsoft.Extensions.DependencyInjection;
using Rystem.Azure.Installation;
using Rystem.Azure.Integration.Cache;
using Rystem.Azure.Integration.Message;
using Rystem.Azure.Integration.Secrets;
using Rystem.Azure.Integration.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Azure
{
    public sealed class AzureFactory
    {
        private readonly Dictionary<string, dynamic> Integrations = new();
        public RedisCacheIntegration RedisCache(string key = "") 
            => Get<RedisCacheIntegration>("RedisCache", key, () => new(AzureManager.Instance.RedisCaches[key]));
        public EventHubIntegration EventHub(EventHubConfiguration eventHubConfiguration, string key = "") 
            => Get<EventHubIntegration>("EventHub", key, () => new(eventHubConfiguration, AzureManager.Instance.EventHubs[key]));
        public ServiceBusIntegration ServiceBus(ServiceBusConfiguration serviceBusConfiguration, string key = "") 
            => Get<ServiceBusIntegration>("ServiceBus", key, () => new(serviceBusConfiguration, AzureManager.Instance.ServiceBuses[key]));
        public KeyVaultIntegration KeyVault(string key = "") 
            => Get<KeyVaultIntegration>("KeyVault", key, () => new(AzureManager.Instance.KeyVaults[key]));
        public QueueStorageIntegration QueueStorage(QueueStorageConfiguration queueStorageConfiguration, string key = "")
            => Get<QueueStorageIntegration>("QueueStorage", key, () => new(queueStorageConfiguration, AzureManager.Instance.Storages[key]));
        public BlobStorageIntegration BlobStorage(BlobStorageConfiguration blobStorageConfiguration, string key = "")
            => Get<BlobStorageIntegration>("BlobStorage", key, () => new(blobStorageConfiguration, AzureManager.Instance.Storages[key]));
        public TableStorageIntegration TableStorage(TableStorageConfiguration tableStorageConfiguration, string key = "")
            => Get<TableStorageIntegration>("TableStorage", key, () => new(tableStorageConfiguration, AzureManager.Instance.Storages[key]));
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