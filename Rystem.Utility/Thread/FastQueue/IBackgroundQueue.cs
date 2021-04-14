using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.BackgroundWork
{
    interface IBackgroundQueue<T>
    {
        void AddElement(T entity);
        List<T> DequeueFirstMaxElement();
        int Count();
    }
}