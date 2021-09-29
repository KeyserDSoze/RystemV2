using Rystem.Background;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rystem.Concurrency
{
    internal sealed class Races
    {
        private readonly Dictionary<string, RaceConditionExecutor> RaceConditions = new();
        private Races()
        {

        }
        public static Races Instance { get; } = new();
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
                    foreach (var rc in Instance.RaceConditions)
                        if (rc.Value.IsExpired)
                            removeKeys.Add(rc.Key);
                    foreach (string keyToRemove in removeKeys)
                        Instance.RaceConditions.Remove(keyToRemove);
                }
            };
            return loop.RunInBackgroundAsync(typeof(Races).FullName, () => 1000 * 60 * 60);
        }
        public async Task<RaceConditionResponse> RunAsync(Func<Task> action, string id, TimeSpan timeWindow, IDistributedImplementation distributedImplementation)
        {
            if (!IsBackgroundRunning)
                _ = RunBackgroundManagerAsync();
            if (!RaceConditions.ContainsKey(id))
                lock (Semaphore)
                    if (!RaceConditions.ContainsKey(id))
                        RaceConditions.Add(id, new RaceConditionExecutor(id, timeWindow));
            return await RaceConditions[id].ExecuteAsync(action, distributedImplementation).NoContext();
        }
    }
}