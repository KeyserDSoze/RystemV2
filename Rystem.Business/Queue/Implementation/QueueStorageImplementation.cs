using Rystem.Azure.Integration.Storage;
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

        public async Task<IEnumerable<TEntity>> ReadAsync(int path, int organization)
            => (await Integration.ReadAsync().NoContext()).Select(x => x.FromJson<TEntity>());

        public async Task<bool> SendAsync(TEntity message, int path, int organization)
            => await Integration.SendAsync(message.ToJson());

        public async Task<bool> SendBatchAsync(IEnumerable<TEntity> messages, int path, int organization)
        {
            List<Task> sents = new();
            foreach (var message in messages)
                sents.Add(SendAsync(message, path, organization));
            await Task.WhenAll(sents).NoContext();
            return true;
        }

        public async Task<long> SendScheduledAsync(TEntity message, int delayInSeconds, int path, int organization)
            => await Integration.SendScheduledAsync(message.ToJson(), delayInSeconds);

        public async Task<IEnumerable<long>> SendScheduledBatchAsync(IEnumerable<TEntity> messages, int delayInSeconds, int path, int organization)
        {
            List<Task<long>> sents = new();
            foreach (var message in messages)
                sents.Add(SendScheduledAsync(message, delayInSeconds, path, organization));
            await Task.WhenAll(sents).NoContext();
            return sents.Select(x => x.Result);
        }
    }
}