using Rystem.Azure.Integration.Cache;
using Rystem.Azure.Integration.Message;
using Rystem.Azure.Integration.Secrets;
using Rystem.Azure.Integration.Storage;
using System;
using System.Collections.Generic;

namespace Rystem.Azure
{
    public sealed class AzureFactory
    {
        public AzureManager Manager { get; }
        public AzureFactory(AzureManager manager) => Manager = manager;
        public RedisCacheIntegration RedisCache(string key = "") 
            => new(Manager.RedisCaches[key]);
        public EventHubIntegration EventHub(EventHubConfiguration eventHubConfiguration, string key = "") 
            => new(eventHubConfiguration, Manager.EventHubs[key]);
        public ServiceBusIntegration ServiceBus(ServiceBusConfiguration serviceBusConfiguration, string key = "") 
            => new(serviceBusConfiguration, Manager.ServiceBuses[key]);
        public KeyVaultIntegration KeyVault(string key = "") 
            => new(Manager.KeyVaults[key]);
        public QueueStorageIntegration QueueStorage(QueueStorageConfiguration queueStorageConfiguration, string key = "")
            => new(queueStorageConfiguration, Manager.Storages[key]);
        public BlobStorageIntegration BlobStorage(BlobStorageConfiguration blobStorageConfiguration, string key = "")
            => new(blobStorageConfiguration, Manager.Storages[key]);
        public TableStorageIntegration TableStorage(TableStorageConfiguration tableStorageConfiguration, string key = "")
            => new(tableStorageConfiguration, Manager.Storages[key]);
    }
}