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

        public Task<IEnumerable<TEntity>> ReadAsync(string partitionKey, string rowKey)
            => throw new NotImplementedException("ServiceBus doesn't allow this operation.");

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
    }
}
