using Rystem.Azure.Integration.Storage;
using Rystem.Background;
using Rystem.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rystem.Business.Queue.Implementation
{
    internal class QueueStorageImplementation<TEntity> : IQueueImplementation<TEntity>
    {
        private readonly Type EntityType;
        private readonly QueueStorageIntegration Integration;
        internal QueueStorageImplementation(QueueStorageIntegration integration, Type entityType)
        {
            Integration = integration;
            this.EntityType = entityType;
        }

        public Task<bool> CleanAsync()
            => throw new NotImplementedException("Queue storage doesn't allow this operation.");

        public Task<bool> DeleteScheduledAsync(long messageId)
            => throw new NotImplementedException("Queue storage doesn't allow this operation.");

        public string GetName()
           => this.Integration.Configuration.Name;
        private readonly string ListenerId = $"{nameof(QueueStorageImplementation<TEntity>)}{Guid.NewGuid():N}";
        private bool IsListening = false;
        public Task ListenAsync(Func<TEntity, string, object, Task> callback, Func<Exception, Task> onErrorCallback)
        {
            if (!IsListening)
            {
                IsListening = true;
                BackgroundWork.Run(async () =>
                {
                    try
                    {
                        var elements = await Integration.ReadAsync().NoContext();
                        List<Task> events = new();
                        foreach (var (Message, Raw) in elements)
                            events.Add(callback(Message.FromJson<TEntity>(), string.Empty, Raw));
                        await Task.WhenAll(events).NoContext();
                        if (onErrorCallback != default)
                            foreach (var task in events)
                                if (task.IsFaulted)
                                    await onErrorCallback(task.Exception).NoContext();
                    }
                    catch (Exception ex)
                    {
                        if (onErrorCallback != default)
                            await onErrorCallback(ex).NoContext();
                    }
                }, ListenerId, () => 120 * 10);
            }
            return Task.CompletedTask;
        }
        public Task StopListenAsync()
        {
            IsListening = false;
            BackgroundWork.Stop(ListenerId);
            return Task.CompletedTask;
        }
        public async Task<IEnumerable<TEntity>> ReadAsync(string partitionKey, string rowKey)
            => (await Integration.ReadAsync().NoContext()).Select(x => x.Message.FromJson<TEntity>());

        public async Task<bool> SendAsync(TEntity message, string partitionKey, string rowKey)
            => await Integration.SendAsync(message.ToJson());

        public async Task<bool> SendBatchAsync(IEnumerable<TEntity> messages, string partitionKey, string rowKey)
        {
            List<Task> sents = new();
            foreach (var message in messages)
                sents.Add(SendAsync(message, partitionKey, rowKey));
            await Task.WhenAll(sents).NoContext();
            return true;
        }

        public async Task<long> SendScheduledAsync(TEntity message, int delayInSeconds, string partitionKey, string rowKey)
            => await Integration.SendScheduledAsync(message.ToJson(), delayInSeconds);

        public async Task<IEnumerable<long>> SendScheduledBatchAsync(IEnumerable<TEntity> messages, int delayInSeconds, string partitionKey, string rowKey)
        {
            List<Task<long>> sents = new();
            foreach (var message in messages)
                sents.Add(SendScheduledAsync(message, delayInSeconds, partitionKey, rowKey));
            await Task.WhenAll(sents).NoContext();
            return sents.Select(x => x.Result);
        }
    }
}