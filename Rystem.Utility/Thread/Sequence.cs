using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.BackgroundWork
{
    public static class Sequence
    {
        public static void Create<T>(int maximumBuffer, TimeSpan maximumRetention, Func<IEnumerable<T>, Task> action, string id = "", QueueType type = QueueType.FirstInFirstOut)
            => Sequences.Instance.Create(id, maximumBuffer, maximumRetention, action, type);
        public static void Flush(string id = "", bool force = false)
            => Sequences.Instance.Flush(id, force);
        public static void Destroy(string id = "")
            => Sequences.Instance.Destroy(id);
        public static void Enqueue<T>(this T entity, string id = "")
            => Sequences.Instance.AddElement(entity, id);
    }
}
