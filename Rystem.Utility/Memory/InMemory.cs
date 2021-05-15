using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Rystem.Memory
{
    internal static class InMemory<T>
    {
        private record InMemoryInstance(DateTime ExpiringTime, object Instance);
        private readonly static ConcurrentDictionary<string, InMemoryInstance> Instances = new();
        public static T Instance(string key)
        {
            if (Instances.ContainsKey(key))
            {
                var instance = Instances[key];
                if (instance.ExpiringTime > DateTime.UtcNow)
                    return (T)instance.Instance;
            }
            return default;
        }
        public static void Update(string key, T value, TimeSpan expireAfter)
        {
            DateTime expiringTime = DateTime.MaxValue;
            if (expireAfter != default)
                expiringTime = DateTime.UtcNow.Add(expireAfter);
            if (!Instances.ContainsKey(key))
                Instances.TryAdd(key, new InMemoryInstance(expiringTime, value));
            else
                Instances[key] = new InMemoryInstance(expiringTime, value);
        }
        public static bool Exists(string key)
            => Instances.ContainsKey(key) && Instances[key].ExpiringTime > DateTime.UtcNow;

        public static T Delete(string key)
        {
            var hasRemoved = Instances.TryRemove(key, out InMemoryInstance instance);
            if (hasRemoved)
                return (T)instance.Instance;
            else
                return default;
        }
        public static IEnumerable<string> List()
            => Instances.Keys.Select(x => x);
    }
}