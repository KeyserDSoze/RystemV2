using Rystem.Azure.Integration.Message;
using Rystem.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Business.Queue.Implementation
{
    internal class EventHubImplementation<TEntity> : IQueueImplementation<TEntity>
    {
        private readonly Type EntityType;
        private readonly EventHubIntegration Integration;
        internal EventHubImplementation(EventHubIntegration integration, Type entityType)
        {
            Integration = integration;
            this.EntityType = entityType;
        }

        public Task<bool> CleanAsync()
            => throw new NotImplementedException("Event hub doesn't allow this operation.");
        public Task<bool> DeleteScheduledAsync(long messageId)
            => throw new NotImplementedException("Event hub doesn't allow this operation.");

        public string GetName()
           => this.Integration.Configuration.Name;

        public Task<IEnumerable<TEntity>> ReadAsync(int path, int organization)
            => throw new NotImplementedException("Event hub doesn't allow this operation.");

        public async Task<bool> SendAsync(TEntity message, int path, int organization)
        {
            await Integration.SendAsync(message.ToJson(), path > 0 ? path.ToString() : null, organization > 0 ? organization.ToString() : null).NoContext();
            return true;
        }
        public async Task<bool> SendBatchAsync(IEnumerable<TEntity> messages, int path, int organization)
        {
            await Integration.SendBatchAsync(messages.Select(x => x.ToJson()), path > 0 ? path.ToString() : null, organization > 0 ? organization.ToString() : null).NoContext();
            return true;
        }
        public Task<long> SendScheduledAsync(TEntity message, int delayInSeconds, int path, int organization)
            => throw new NotImplementedException("Event hub doesn't allow this operation.");
        public Task<IEnumerable<long>> SendScheduledBatchAsync(IEnumerable<TEntity> messages, int delayInSeconds, int path, int organization)
            => throw new NotImplementedException("Event hub doesn't allow this operation.");
    }
}
