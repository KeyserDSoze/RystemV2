using Rystem.Memory;
using System.Collections.Generic;

namespace System
{
    public static class InMemoryExtensions
    {
        public static T Instance<T>(this Key key)
            => InMemory<T>.Instance(key.Value);
        public static T Remove<T>(this Key key)
            => InMemory<T>.Delete(key.Value);
        public static bool Exists<T>(this Key key)
            => InMemory<T>.Exists(key.Value);
        public static void Update<T>(this Key key, T instance, TimeSpan expireAfter = default)
            => InMemory<T>.Update(key.Value, instance, expireAfter);
        public static IEnumerable<string> List<T>(this Key key)
            => InMemory<T>.List();
    }
}