using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.BackgroundWork
{
    internal class BackgroundStack<T> : IBackgroundQueue<T>
    {
        private readonly Stack<T> Queues = new();
        private static readonly object Semaphore = new();
        public void AddElement(T entity)
        {
            lock (Semaphore)
                Queues.Push(entity);
        }
        public int Count() => Queues.Count;
        public List<T> DequeueFirstMaxElement()
        {
            List<T> entities = new();
            lock (Semaphore)
            {
                int count = Queues.Count;
                for (int i = 0; i < count; i++)
                    entities.Add(Queues.Pop());
            }
            return entities;
        }
    }
}