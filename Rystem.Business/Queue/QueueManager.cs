using Rystem.Azure;
using Rystem.Business.Queue.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rystem.Business
{
    internal sealed class QueueManager<TEntity> : IQueueManager<TEntity>
    {
        private readonly IDictionary<Installation, IQueueImplementation<TEntity>> Implementations = new Dictionary<Installation, IQueueImplementation<TEntity>>();
        private readonly IDictionary<Installation, ProvidedService> QueueConfigurations;
        private readonly AzureManager Manager;
        public QueueManager(Options<IQueueManager<TEntity>> options, AzureManager manager)
        {
            QueueConfigurations = options.Services;
            Manager = manager;
            foreach(var conf in QueueConfigurations)
            {
                ProvidedService configuration = QueueConfigurations[conf.Key];
                switch (configuration.Type)
                {
                    case ServiceProviderType.AzureQueueStorage:
                        Implementations.Add(conf.Key, new QueueStorageImplementation<TEntity>(Manager.QueueStorage(configuration.Configurations, configuration.ServiceKey)));
                        break;
                    case ServiceProviderType.AzureEventHub:
                        Implementations.Add(conf.Key, new EventHubImplementation<TEntity>(Manager.EventHub(configuration.Configurations, configuration.ServiceKey)));
                        break;
                    case ServiceProviderType.AzureServiceBus:
                        Implementations.Add(conf.Key, new ServiceBusImplementations<TEntity>(Manager.ServiceBus(configuration.Configurations, configuration.ServiceKey)));
                        break;
                    default:
                        throw new InvalidOperationException($"Wrong type installed {configuration.Type}");
                }
            }
        }
        public async Task<bool> SendAsync(TEntity message, Installation installation, string partitionKey, string rowKey)
            => await Implementations[installation].SendAsync(message, partitionKey, rowKey).NoContext();
        public async Task<long> SendScheduledAsync(TEntity message, int delayInSeconds, Installation installation, string partitionKey, string rowKey)
            => await Implementations[installation].SendScheduledAsync(message, delayInSeconds, partitionKey, rowKey).NoContext();
        public async Task<bool> DeleteScheduledAsync(long messageId, Installation installation)
            => await Implementations[installation].DeleteScheduledAsync(messageId).NoContext();
        public async Task<bool> SendBatchAsync(IEnumerable<TEntity> messages, Installation installation, string partitionKey, string rowKey)
            => await Implementations[installation].SendBatchAsync(messages.Select(x => x), partitionKey, rowKey).NoContext();
        public async Task<IEnumerable<long>> SendScheduledBatchAsync(IEnumerable<TEntity> messages, int delayInSeconds, Installation installation, string partitionKey, string rowKey)
            => await Implementations[installation].SendScheduledBatchAsync(messages.Select(x => x), delayInSeconds, partitionKey, rowKey).NoContext();
        public async Task<IEnumerable<TEntity>> ReadAsync(Installation installation, string partitionKey, string rowKey)
           => await Implementations[installation].ReadAsync(partitionKey, rowKey).NoContext();
        public async Task ListenAsync(Func<TEntity, string, object, Task> callback, Func<Exception, Task> onErrorCallback, Installation installation)
            => await Implementations[installation].ListenAsync(callback, onErrorCallback).NoContext();
        public async Task StopListenAsync(Installation installation)
            => await Implementations[installation].StopListenAsync().NoContext();
        public async Task<bool> CleanAsync(Installation installation)
            => await Implementations[installation].CleanAsync().NoContext();
        public string GetName(Installation installation) => Implementations[installation].GetName();
        public async Task<bool> WarmUpAsync()
        {
            List<Task> tasks = new();
            foreach (var configuration in QueueConfigurations)
                tasks.Add(Implementations[configuration.Key].WarmUpAsync());
            await Task.WhenAll(tasks).NoContext();
            return true;
        }
    }
}