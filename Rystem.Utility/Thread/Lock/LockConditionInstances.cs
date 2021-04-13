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