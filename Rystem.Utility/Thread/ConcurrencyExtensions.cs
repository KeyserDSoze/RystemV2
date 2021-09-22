using System;
using System.Threading.Tasks;

namespace Rystem.Concurrency
{
    public static class ConcurrencyExtensions
    {
        /// <summary>
        /// Deal with concurrency and allow a running queue. Each action runs only after the previous running action ends.
        /// </summary>
        /// <param name="task">Action to perform</param>
        /// <param name="lockId">Lock key, task with different id doesn't partecipate at the same lock chain.</param>
        /// <param name="distributedImplementation">A implementation that overrides the internal lock check function.</param>
        /// <returns></returns>
        public static async Task<LockResponse> LockAsync(this Func<Task> task, string lockId = "", IDistributedImplementation distributedImplementation = default)
            => await Locks.Instance.RunAsync(task, lockId, distributedImplementation).NoContext();
        /// <summary>
        /// Deal with concurrency and allow only one method to run. Other concurrent task will be dropped.
        /// </summary>
        /// <param name="task">Action to perform</param>
        /// <param name="raceId">Concurrency key, task with different id doesn't partecipate at the same race.</param>
        /// <param name="distributedImplementation">A implementation that overrides the internal lock check function.</param>
        /// <returns></returns>
        public static async Task<RaceConditionResponse> RunUnderRaceConditionAsync(this Func<Task> task, string raceId = "", TimeSpan timeWindow = default, IDistributedImplementation distributedImplementation = default)
            => await Races.Instance.RunAsync(task, raceId, timeWindow, distributedImplementation).NoContext();
    }
}