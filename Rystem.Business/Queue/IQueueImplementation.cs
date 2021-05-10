using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Business.Queue
{
    internal interface IQueueImplementation<TEntity>
    {
        Task<bool> SendAsync(TEntity message, string partitionKey, string rowKey);
        Task<long> SendScheduledAsync(TEntity message, int delayInSeconds, string partitionKey, string rowKey);
        Task<bool> DeleteScheduledAsync(long messageId);
        Task<bool> SendBatchAsync(IEnumerable<TEntity> messages, string partitionKey, string rowKey);
        Task<IEnumerable<long>> SendScheduledBatchAsync(IEnumerable<TEntity> messages, int delayInSeconds, string partitionKey, string rowKey);
        Task<IEnumerable<TEntity>> ReadAsync(string partitionKey, string rowKey);
        Task<bool> CleanAsync();
        string GetName();
    }
}
