using System.Collections.Generic;

namespace Rystem.Background
{
    interface IBackgroundQueue<T>
    {
        void AddElement(T entity);
        List<T> DequeueFirstMaxElement();
        int Count();
    }
}