using System;
using System.Threading.Tasks;

namespace Rystem.Concurrency
{
    public static class LockExtensions
    {
        /// <summary>
        /// Deal with concurrency and allow only one method to run. Other concurrent task will be dropped.
        /// </summary>
        /// <param name="task">Action to perform</param>
        /// <param name="lockId">Lock key, task with different id doesn't partecipate at the same lock chain.</param>
        /// <param name="distributedImplementation">A implementation that overrides the internal lock check function.</param>
        /// <returns></returns>
        public static async Task<LockResponse> RunAsync(this Func<Task> task, string lockId = "", IDistributedImplementation distributedImplementation = default)
            => await Locks.Instance.RunAsync(task, lockId, distributedImplementation).NoContext();
    }
}
