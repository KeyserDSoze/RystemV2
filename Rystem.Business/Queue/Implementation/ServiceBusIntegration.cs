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

        public Task<IEnumerable<TEntity>> ReadAsync(int path, int organization)
            => throw new NotImplementedException("ServiceBus doesn't allow this operation.");

        public async Task<bool> SendAsync(TEntity message, int path, int organization)
        {
            await Integration.SendAsync(message.ToJson()).NoContext();
            return true;
        }

        public async Task<bool> SendBatchAsync(IEnumerable<TEntity> messages, int path, int organization)
        {
            await Integration.SendBatchAsync(messages.Select(x => x.ToJson())).NoContext();
            return true;
        }

        public async Task<long> SendScheduledAsync(TEntity message, int delayInSeconds, int path, int organization)
            => await Integration.SendAsync(message.ToJson(), delayInSeconds).NoContext();

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
