using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Rystem.Background
{
    internal sealed class BackgroundStack<T> : IBackgroundQueue<T>
    {
        private readonly ConcurrentStack<T> Queues = new();
        public void AddElement(T entity)
            => Queues.Push(entity);
        public int Count() => Queues.Count;
        public IEnumerable<T> DequeueFirstMaxElement()
        {
            int count = Queues.Count;
            T[] array = new T[count];
            Queues.TryPopRange(array, 0, count);
            return array;
        }
    }
}