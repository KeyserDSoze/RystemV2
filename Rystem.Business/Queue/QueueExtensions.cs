using Rystem;
using Rystem.Business;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System
{
    public static partial class QueueExtensions
    {
        private static IQueueManager<TEntity> Manager<TEntity>(this TEntity entity)
            where TEntity : IQueue
            => ServiceLocator.GetService<IQueueManager<TEntity>>() ??
                ConfigurableManagerHelper<TEntity, IQueueManager<TEntity>, RystemQueueServiceProvider>.ManagerToConfigure(entity);

        public static async Task<bool> SendAsync<TEntity>(this TEntity message, string partitionKey = default, string rowKey = default, Installation installation = Installation.Default)
            where TEntity : IQueue
            => await message.Manager().SendAsync(message, installation, partitionKey, rowKey).NoContext();
        public static async Task<long> SendScheduledAsync<TEntity>(this TEntity message, int delayInSeconds, string partitionKey = default, string rowKey = default, Installation installation = Installation.Default)
            where TEntity : IQueue
           => await message.Manager().SendScheduledAsync(message, delayInSeconds, installation, partitionKey, rowKey).NoContext();
        public static async Task<bool> DeleteScheduledAsync<TEntity>(this TEntity message, long messageId, Installation installation = Installation.Default)
            where TEntity : IQueue
            => await message.Manager().DeleteScheduledAsync(messageId, installation).NoContext();
        public static async Task<bool> SendBatchAsync<TEntity>(this IEnumerable<TEntity> messages, string partitionKey = default, string rowKey = default, Installation installation = Installation.Default)
            where TEntity : IQueue
        {
            bool result = true;
            foreach (var msgs in messages.GroupBy(x => x.GetType().FullName))
                result &= await msgs.FirstOrDefault().Manager().SendBatchAsync(msgs, installation, partitionKey, rowKey).NoContext();
            return result;
        }
        public static async Task<IEnumerable<long>> SendScheduledBatchAsync<TEntity>(this IEnumerable<TEntity> messages, int delayInSeconds, string partitionKey = default, string rowKey = default, Installation installation = Installation.Default)
            where TEntity : IQueue
        {
            List<long> aggregatedResponse = new List<long>();
            foreach (var msgs in messages.GroupBy(x => x.GetType().FullName))
                aggregatedResponse.AddRange(await msgs.FirstOrDefault().Manager().SendScheduledBatchAsync(msgs, delayInSeconds, installation, partitionKey, rowKey).NoContext());
            return aggregatedResponse;
        }
        public static async Task<IEnumerable<TEntity>> ReadAsync<TEntity>(this TEntity message, string partitionKey = default, string rowKey = default, Installation installation = Installation.Default)
            where TEntity : IQueue
            => await message.Manager().ReadAsync(installation, partitionKey, rowKey).NoContext();
        public static async Task<bool> CleanAsync<TEntity>(this TEntity message, Installation installation = Installation.Default)
            where TEntity : IQueue
            => await message.Manager().CleanAsync(installation).NoContext();
        public static async Task ListenAsync<TEntity>(this TEntity message, Func<TEntity, string, object, Task> callback, Func<Exception, Task> onErrorCallback = default, Installation installation = Installation.Default)
            where TEntity : IQueue
            => await message.Manager().ListenAsync(callback, onErrorCallback, installation).NoContext();
        public static async Task StopAsync<TEntity>(this TEntity message, Installation installation = Installation.Default)
            where TEntity : IQueue
            => await message.Manager().StopListenAsync(installation).NoContext();
        public static bool Send<TEntity>(this TEntity message, string partitionKey = default, string rowKey = default, Installation installation = Installation.Default)
            where TEntity : IQueue
           => message.SendAsync(partitionKey, rowKey, installation).ToResult();
        public static long SendScheduled<TEntity>(this TEntity message, int delayInSeconds, string partitionKey = default, string rowKey = default, Installation installation = Installation.Default)
            where TEntity : IQueue
            => message.SendScheduledAsync(delayInSeconds, partitionKey, rowKey, installation).ToResult();
        public static bool DeleteScheduled<TEntity>(this TEntity message, long messageId, Installation installation = Installation.Default)
            where TEntity : IQueue
            => message.DeleteScheduledAsync(messageId, installation).ToResult();
        public static bool SendBatch<TEntity>(this IEnumerable<TEntity> messages, string partitionKey = default, string rowKey = default, Installation installation = Installation.Default)
            where TEntity : IQueue
            => messages.SendBatchAsync(partitionKey, rowKey, installation).ToResult();
        public static IEnumerable<long> SendScheduledBatch<TEntity>(this IEnumerable<TEntity> messages, int delayInSeconds, string partitionKey = default, string rowKey = default, Installation installation = Installation.Default)
            where TEntity : IQueue
            => messages.SendScheduledBatchAsync(delayInSeconds, partitionKey, rowKey, installation).ToResult();
        public static IEnumerable<TEntity> Read<TEntity>(this TEntity message, string partitionKey = default, string rowKey = default, Installation installation = Installation.Default)
            where TEntity : IQueue
            => message.ReadAsync(partitionKey, rowKey, installation).ToResult();
        public static bool Clean<TEntity>(this TEntity message, Installation installation = Installation.Default)
            where TEntity : IQueue
            => message.CleanAsync(installation).ToResult();
        public static void Listen<TEntity>(this TEntity message, Func<TEntity, string, object, Task> callback, Func<Exception, Task> onErrorCallback = default, Installation installation = Installation.Default)
           where TEntity : IQueue
           => message.ListenAsync(callback, onErrorCallback, installation).ToResult();
        public static void Stop<TEntity>(this TEntity message, Installation installation = Installation.Default)
            where TEntity : IQueue
            => message.StopAsync(installation).ToResult();
        public static string GetName<TEntity>(this TEntity message, Installation installation = Installation.Default)
            where TEntity : IQueue
            => message.Manager().GetName(installation);
    }
}