using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.BackgroundWork
{
    public record SequenceProperty<T>(string Name = "", int MaximumBuffer = 5000, TimeSpan MaximumRetention = default, params Func<IEnumerable<T>, Task>[] Actions) : Configuration(Name)
    {
        public SequenceProperty() : this(default) { }
    }
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
        public void Create<T>(SequenceProperty<T> property, QueueType type)
        {
            if (!Queues.ContainsKey(property.Name))
                lock (Semaphore)
                    if (!Queues.ContainsKey(property.Name))
                        switch (type)
                        {
                            default:
                            case QueueType.FirstInFirstOut:
                                Queues.Add(property.Name, new QueueContainer<T>(property, new BackgroundQueue<T>()));
                                break;
                            case QueueType.LastInFirstOut:
                                Queues.Add(property.Name, new QueueContainer<T>(property, new BackgroundStack<T>()));
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
            private readonly SequenceProperty<T> Property;
            private DateTime ExpiringTime;
            public bool IsExpired => DateTime.UtcNow >= ExpiringTime;
            public QueueContainer(SequenceProperty<T> property, IBackgroundQueue<T> queue)
            {
                Property = property;
                Queue = queue;
                ExpiringTime = DateTime.UtcNow.Add(Property.MaximumRetention);
            }
            public bool Add(object entity)
            {
                Queue.AddElement((T)entity);
                return Queue.Count() >= Property.MaximumBuffer || IsExpired;
            }
            public void Invoke()
            {
                ExpiringTime = DateTime.UtcNow.Add(Property.MaximumRetention);
                System.Threading.ThreadPool.QueueUserWorkItem(async (x) =>
                {
                    if (Property.Actions != default)
                        foreach (var action in Property.Actions)
                            await action.Invoke(Queue.DequeueFirstMaxElement()).NoContext();
                });
            }
        }
    }
}