using System;
using System.Threading.Tasks;

namespace Rystem.Concurrency
{
    public static class RaceCondition
    {
        /// <summary>
        /// Deal with concurrency and allow only one method to run. Other concurrent task will be dropped.
        /// </summary>
        /// <param name="task">Action to perform</param>
        /// <param name="raceId">Concurrency key, task with different id doesn't partecipate at the same race.</param>
        /// <param name="distributedImplementation">A implementation that overrides the internal lock check function.</param>
        /// <returns></returns>
        public static async Task<RaceConditionResponse> RunAsync(Func<Task> task, string raceId = "", TimeSpan timeWindow = default, IDistributedImplementation distributedImplementation = default)
            => await task.RunUnderRaceConditionAsync(raceId, timeWindow, distributedImplementation).NoContext();
        public static RaceConditionResponse Run(Action action, string raceId = "", TimeSpan timeWindow = default, IDistributedImplementation distributedImplementation = default)
            => action.RunUnderRaceCondition(raceId, timeWindow, distributedImplementation);
    }
}
