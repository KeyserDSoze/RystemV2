using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rystem.Business
{
    public interface IQueueManager<TEntity> : IWarmUp
    {
        Task<bool> SendAsync(TEntity message, Installation installation, string partitionKey, string rowKey);
        Task<long> SendScheduledAsync(TEntity message, int delayInSeconds, Installation installation, string partitionKey, string rowKey);
        Task<bool> DeleteScheduledAsync(long messageId, Installation installation);
        Task<bool> SendBatchAsync(IEnumerable<TEntity> messages, Installation installation, string partitionKey, string rowKey);
        Task<IEnumerable<long>> SendScheduledBatchAsync(IEnumerable<TEntity> messages, int delayInSeconds, Installation installation, string partitionKey, string rowKey);
        Task<IEnumerable<TEntity>> ReadAsync(Installation installation, string partitionKey, string rowKey);
        Task ListenAsync(Func<TEntity, string, object, Task> callback, Func<Exception, Task> onErrorCallback, Installation installation);
        Task StopListenAsync(Installation installation);
        Task<bool> CleanAsync(Installation installation);
        string GetName(Installation installation);
    }
}