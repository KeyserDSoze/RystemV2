﻿using Rystem.BackgroundWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Concurrency
{
    internal sealed class Locks
    {
        private readonly Dictionary<string, LockExecutor> LockConditions = new();
        private Locks()
        {
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
            loop.RunInBackground("Rystem.Background.Locks", 1000 * 60 * 60);
        }
        public static Locks Instance { get; } = new();
        private static readonly object Semaphore = new();
        public async Task<LockResponse> RunAsync(Func<Task> action, string id)
        {
            if (!LockConditions.ContainsKey(id))
                lock (Semaphore)
                    if (!LockConditions.ContainsKey(id))
                        LockConditions.Add(id, new LockExecutor());
            return await LockConditions[id].ExecuteAsync(action).NoContext();
        }
    }
}