using Rystem.Azure.Integration.Message;
using Rystem.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rystem.Business.Queue.Implementation
{
    internal class ServiceBusImplementations<TEntity> : IQueueImplementation<TEntity>
    {
        private readonly Type EntityType;
        private readonly ServiceBusIntegration Integration;
        internal ServiceBusImplementations(ServiceBusIntegration integration, Type entityType)
        {
            Integration = integration;
            this.EntityType = entityType;
        }
        public string GetName()
         => this.Integration.Configuration.Name;
        public Task<bool> CleanAsync()
            => throw new NotImplementedException("ServiceBus doesn't allow this operation.");
        public async Task<bool> DeleteScheduledAsync(long messageId)
        {
            await Integration.RemoveScheduledAsync(messageId).NoContext();
            return true;
        }
        private List<(TEntity Entity, string PartitionKey, object Item)> Entities = new();
        private readonly object TrafficLight = new();
        private Task Add(TEntity entity, string partitionKey, object item)
        {
            Entities.Add((entity, partitionKey, item));
            return Task.CompletedTask;
        }
        public async Task<IEnumerable<TEntity>> ReadAsync(string partitionKey, string rowKey)
        {
            if (!IsListening)
                await ListenAsync(Add, default).NoContext();
            lock (TrafficLight)
            {
                var entities = partitionKey == null ? Entities : Entities.Where(x => x.PartitionKey == partitionKey);
                if (partitionKey == null)
                    Entities = new();
                else
                    Entities = Entities.Where(x => x.PartitionKey != partitionKey).ToList();
                return entities.Select(x => x.Entity);
            }
        }
        public async Task<bool> SendAsync(TEntity message, string partitionKey, string rowKey)
        {
            await Integration.SendAsync(message.ToJson()).NoContext();
            return true;
        }
        public async Task<bool> SendBatchAsync(IEnumerable<TEntity> messages, string partitionKey, string rowKey)
        {
            await Integration.SendBatchAsync(messages.Select(x => x.ToJson())).NoContext();
            return true;
        }
        public async Task<long> SendScheduledAsync(TEntity message, int delayInSeconds, string partitionKey, string rowKey)
            => await Integration.SendAsync(message.ToJson(), delayInSeconds).NoContext();
        public async Task<IEnumerable<long>> SendScheduledBatchAsync(IEnumerable<TEntity> messages, int delayInSeconds, string partitionKey, string rowKey)
        {
            List<Task<long>> sents = new();
            foreach (var message in messages)
                sents.Add(SendScheduledAsync(message, delayInSeconds, partitionKey, rowKey));
            await Task.WhenAll(sents).NoContext();
            return sents.Select(x => x.Result);
        }
        private bool IsListening = false;
        public async Task ListenAsync(Func<TEntity, string, object, Task> callback, Func<Exception, Task> onErrorCallback)
        {
            if (!IsListening)
            {
                IsListening = true;
                await Integration.StartReadAsync(
                    async (x) =>
                    {
                        try
                        {
                            await callback(x.Message.Body.ToObjectFromJson<TEntity>(), x.Message.PartitionKey, x.Message).NoContext();
                        }
                        catch (Exception ex)
                        {
                            if (onErrorCallback != default)
                                await onErrorCallback(ex).NoContext();
                        }
                    },
                    async x =>
                    {
                        if (onErrorCallback != default)
                            await onErrorCallback(x.Exception);
                    }
                ).NoContext();
            }
        }
        public Task StopListenAsync()
        {
            IsListening = false;
            return Integration.StopReadAsync();
        }
    }
}