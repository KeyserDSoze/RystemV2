using Rystem.Azure.Installation;
using Rystem.Azure.Integration.Message;
using Rystem.Azure.Integration.Storage;
using Rystem.Business.Queue.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Business.Queue
{
    internal sealed class QueueManager<TEntity>
        where TEntity : IQueue
    {
        private readonly IDictionary<Installation, IQueueImplementation<TEntity>> Implementations = new Dictionary<Installation, IQueueImplementation<TEntity>>();
        private readonly IDictionary<Installation, ProvidedService> QueueConfiguration;
        private static readonly object TrafficLight = new();
        private IQueueImplementation<TEntity> Implementation(Installation installation)
        {
            if (!Implementations.ContainsKey(installation))
                lock (TrafficLight)
                    if (!Implementations.ContainsKey(installation))
                    {
                        ProvidedService configuration = QueueConfiguration[installation];
                        switch (configuration.Type)
                        {
                            case ServiceProviderType.AzureQueueStorage:
                                Implementations.Add(installation, new QueueStorageImplementation<TEntity>(AzureBuilder.Factory.QueueStorage(configuration.Configurations, configuration.ServiceKey), DefaultEntity));
                                break;
                            case ServiceProviderType.AzureEventHub:
                                Implementations.Add(installation, new EventHubImplementation<TEntity>(AzureBuilder.Factory.EventHub(configuration.Configurations, configuration.ServiceKey), DefaultEntity));
                                break;
                            case ServiceProviderType.AzureServiceBus:
                                Implementations.Add(installation, new ServiceBusImplementations<TEntity>(AzureBuilder.Factory.ServiceBus(configuration.Configurations, configuration.ServiceKey), DefaultEntity));
                                break;
                            default:
                                throw new InvalidOperationException($"Wrong type installed {configuration.Type}");
                        }
                    }
            return Implementations[installation];
        }
        private readonly Type DefaultEntity;
        public QueueManager(RystemQueueServiceProvider serviceProvider)
        {
            QueueConfiguration = serviceProvider.Services.ToDictionary(x => x.Key, x => x.Value);
            DefaultEntity = serviceProvider.InstanceType;
        }
        public async Task<bool> SendAsync(TEntity message, Installation installation, string partitionKey, string rowKey)
            => await Implementation(installation).SendAsync(message, partitionKey, rowKey).NoContext();
        public async Task<long> SendScheduledAsync(TEntity message, int delayInSeconds, Installation installation, string partitionKey, string rowKey)
            => await Implementation(installation).SendScheduledAsync(message, delayInSeconds, partitionKey, rowKey).NoContext();
        public async Task<bool> DeleteScheduledAsync(long messageId, Installation installation)
            => await Implementation(installation).DeleteScheduledAsync(messageId).NoContext();
        public async Task<bool> SendBatchAsync(IEnumerable<TEntity> messages, Installation installation, string partitionKey, string rowKey)
            => await Implementation(installation).SendBatchAsync(messages.Select(x => x), partitionKey, rowKey).NoContext();
        public async Task<IEnumerable<long>> SendScheduledBatchAsync(IEnumerable<TEntity> messages, int delayInSeconds, Installation installation, string partitionKey, string rowKey)
            => await Implementation(installation).SendScheduledBatchAsync(messages.Select(x => x), delayInSeconds, partitionKey, rowKey).NoContext();
        public async Task<IEnumerable<TEntity>> ReadAsync(Installation installation, string partitionKey, string rowKey)
           => await Implementation(installation).ReadAsync(partitionKey, rowKey).NoContext();
        public async Task ListenAsync(Func<TEntity, string, object, Task> callback, Func<Exception, Task> onErrorCallback, Installation installation)
            => await Implementation(installation).ListenAsync(callback, onErrorCallback).NoContext();
        public async Task StopListenAsync(Installation installation)
            => await Implementation(installation).StopListenAsync().NoContext();
        public async Task<bool> CleanAsync(Installation installation)
            => await Implementation(installation).CleanAsync().NoContext();
        public string GetName(Installation installation) => Implementations[installation].GetName();
    }
}