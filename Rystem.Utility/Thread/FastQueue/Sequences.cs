using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.BackgroundWork
{
    internal sealed class Sequences
    {
        private readonly Dictionary<string, IQueueContainer> Queues = new();
        private Sequences()
        {
            Action loop = () =>
            {
                List<IQueueContainer> containersToRefresh = new();
                lock (Semaphore)
                {
                    foreach (var rc in Instance.Queues)
                        if (rc.Value.IsExpired)
                            containersToRefresh.Add(rc.Value);
                }
                foreach (var toRefresh in containersToRefresh)
                    toRefresh.Invoke();
            };
            loop.RunInBackground($"Rystem.Background.Sequences", 1000 * 60);
        }
        public static Sequences Instance { get; } = new();
        private static readonly object Semaphore = new();
        public void Create<T>(string id, int maximumBuffer, TimeSpan maximumRetention, Func<IEnumerable<T>, Task> action, QueueType type)
        {
            if (!Queues.ContainsKey(id))
                lock (Semaphore)
                    if (!Queues.ContainsKey(id))
                        switch (type)
                        {
                            default:
                            case QueueType.FirstInFirstOut:
                                Queues.Add(id, new QueueContainer<T>(maximumBuffer, maximumRetention, action, new BackgroundQueue<T>()));
                                break;
                            case QueueType.LastInFirstOut:
                                Queues.Add(id, new QueueContainer<T>(maximumBuffer, maximumRetention, action, new BackgroundStack<T>()));
                                break;
                        }
        }
        public void Flush(string id, bool force)
        {
            var queue = Queues[id];
            if (force || queue.IsExpired)
                queue.Invoke();
        }
        public void Destroy(string id)
        {
            IQueueContainer queueContainer = default;
            if (Queues.ContainsKey(id))
                lock (Semaphore)
                    if (Queues.ContainsKey(id))
                    {
                        queueContainer = Queues[id];
                        Queues.Remove(id);
                    }
            if (queueContainer != default)
                queueContainer.Invoke();
        }
        public void AddElement<T>(T element, string queueId)
        {
            if (!Queues.ContainsKey(queueId))
                throw new ArgumentException($"{queueId} not found. Please install before using, use Queue.Create method.");
            var queue = Queues[queueId];
            if (queue.Add(element))
                queue.Invoke();
        }
        private interface IQueueContainer
        {
            bool IsExpired { get; }
            bool Add(object entity);
            void Invoke();
        }
        private class QueueContainer<T> : IQueueContainer
        {
            public IBackgroundQueue<T> Queue { get; }
            private readonly int MaximumBuffer;
            private readonly TimeSpan MaximumRetention;
            private DateTime ExpiringTime;
            public Func<IEnumerable<T>, Task> Action { get; }
            public bool IsExpired => DateTime.UtcNow >= ExpiringTime;
            public QueueContainer(int maximumBuffer, TimeSpan maximumRetention, Func<IEnumerable<T>, Task> action, IBackgroundQueue<T> queue)
            {
                MaximumBuffer = maximumBuffer;
                MaximumRetention = maximumRetention;
                Action = action;
                Queue = queue;
                ExpiringTime = DateTime.UtcNow.Add(MaximumRetention);
            }
            public bool Add(object entity)
            {
                Queue.AddElement((T)entity);
                return Queue.Count() >= MaximumBuffer || IsExpired;
            }
            public void Invoke()
            {
                ExpiringTime = DateTime.UtcNow.Add(MaximumRetention);
                System.Threading.ThreadPool.QueueUserWorkItem(async (x) =>
                {
                    await Action.Invoke(Queue.DequeueFirstMaxElement());
                });
            }
        }
    }
}