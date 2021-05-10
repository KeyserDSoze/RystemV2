using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Business.Queue
{
    internal interface IQueueImplementation<TEntity>
    {
        Task<bool> SendAsync(TEntity message, int path, int organization);
        Task<long> SendScheduledAsync(TEntity message, int delayInSeconds, int path, int organization);
        Task<bool> DeleteScheduledAsync(long messageId);
        Task<bool> SendBatchAsync(IEnumerable<TEntity> messages, int path, int organization);
        Task<IEnumerable<long>> SendScheduledBatchAsync(IEnumerable<TEntity> messages, int delayInSeconds, int path, int organization);
        Task<IEnumerable<TEntity>> ReadAsync(int path, int organization);
        Task<bool> CleanAsync();
        string GetName();
    }
}
