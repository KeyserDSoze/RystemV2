using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Utility.Thread
{
    internal sealed class RaceConditionInstances
    {
        private readonly Dictionary<string, RaceCondition> RaceConditions = new();
        private RaceConditionInstances() { }
        static RaceConditionInstances()
        {
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
            loop.RunInBackground("Rystem.Background.RaceCondition", 1000 * 60 * 60);
        }
        public static RaceConditionInstances Instance { get; } = new();
        private static readonly object Semaphore = new();
        public async Task<RaceConditionResponse> RunAsync(Func<Task> action, string id)
        {
            if (!RaceConditions.ContainsKey(id))
                lock (Semaphore)
                    if (!RaceConditions.ContainsKey(id))
                        RaceConditions.Add(id, new RaceCondition());
            return await RaceConditions[id].ExecuteAsync(action).NoContext();
        }
    }
}