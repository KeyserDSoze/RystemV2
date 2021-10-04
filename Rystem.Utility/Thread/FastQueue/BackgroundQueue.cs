using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Rystem.Background
{
    internal sealed class BackgroundQueue<T> : IBackgroundQueue<T>
    {
        private readonly ConcurrentQueue<T> Queues = new();
        public void AddElement(T entity)
            => Queues.Enqueue(entity);
        public int Count()
            => Queues.Count;
        public IEnumerable<T> DequeueFirstMaxElement()
        {
            List<T> entities = new();
            int count = Queues.Count;
            for (int i = 0; i < count; i++)
            {
                Queues.TryDequeue(out T value);
                if (value != null)
                    entities.Add(value);
            }
            return entities;
        }
    }
}