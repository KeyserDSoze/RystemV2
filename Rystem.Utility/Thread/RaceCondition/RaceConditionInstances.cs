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