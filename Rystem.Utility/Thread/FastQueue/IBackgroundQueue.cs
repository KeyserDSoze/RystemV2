using System.Collections.Generic;

namespace Rystem.Background
{
    interface IBackgroundQueue<T>
    {
        void AddElement(T entity);
        IEnumerable<T> DequeueFirstMaxElement();
        int Count();
    }
}