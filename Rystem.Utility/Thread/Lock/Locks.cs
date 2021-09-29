using Rystem.Background;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rystem.Concurrency
{
    internal sealed class Locks
    {
        private readonly Dictionary<string, LockExecutor> LockConditions = new();
        private Locks()
        {
        }
        public static Locks Instance { get; } = new();
        private static readonly object Semaphore = new();
        private bool IsBackgroundRunning;
        private Task RunBackgroundManagerAsync()
        {
            IsBackgroundRunning = true;
            Action loop = () =>
            {
                List<string> removeKeys = new();
                lock (Semaphore)
                {
                    foreach (var rc in Instance.LockConditions)
                        if (rc.Value.IsExpired)
                            removeKeys.Add(rc.Key);
                    foreach (string keyToRemove in removeKeys)
                        Instance.LockConditions.Remove(keyToRemove);
                }
            };
            return loop.RunInBackgroundAsync(typeof(Locks).FullName, () => 1000 * 60 * 60);
        }
        public async Task<LockResponse> RunAsync(Func<Task> action, string id, IDistributedImplementation implementation)
        {
            if (!IsBackgroundRunning)
                _ = RunBackgroundManagerAsync();
            if (!LockConditions.ContainsKey(id))
                lock (Semaphore)
                    if (!LockConditions.ContainsKey(id))
                        LockConditions.Add(id, new LockExecutor(id));
            return await LockConditions[id].ExecuteAsync(action, implementation).NoContext();
        }
    }
}