using System;
using System.Threading.Tasks;

namespace Rystem.Concurrency
{
    public static class Lock
    {
        /// <summary>
        /// Deal with concurrency and allow a running queue. Each action runs only after the previous running action ends.
        /// </summary>
        /// <param name="task">Action to perform</param>
        /// <param name="lockId">Lock key, task with different id doesn't partecipate at the same lock chain.</param>
        /// <param name="distributedImplementation">A implementation that overrides the internal lock check function.</param>
        /// <returns></returns>
        public static async Task<LockResponse> RunAsync(Func<Task> task, string lockId = "", IDistributedImplementation distributedImplementation = default)
            => await Locks.Instance.RunAsync(task, lockId, distributedImplementation).NoContext();
    }
}
