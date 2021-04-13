using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Utility.Thread
{
    internal sealed class LockConditionInstances
    {
        private readonly Dictionary<string, LockCondition> LockConditions = new();
        private LockConditionInstances() { }
        static LockConditionInstances()
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
            loop.RunInBackground("Rystem.Background.LockCondition", 1000 * 60 * 60);
        }
        public static LockConditionInstances Instance { get; } = new();
        private static readonly object Semaphore = new();
        public async Task<LockConditionResponse> RunAsync(Func<Task> action, string id)
        {
            if (!LockConditions.ContainsKey(id))
                lock (Semaphore)
                    if (!LockConditions.ContainsKey(id))
                        LockConditions.Add(id, new LockCondition());
            return await LockConditions[id].ExecuteAsync(action).NoContext();
        }
    }
}