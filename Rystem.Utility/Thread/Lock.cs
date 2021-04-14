using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Concurrency
{
    public static class Lock
    {
        /// <summary>
        /// Deal with concurrency and allow only one method to run. Other concurrent task will be dropped.
        /// </summary>
        /// <param name="task">Action to perform</param>
        /// <param name="lockId">Lock key, task with different id doesn't partecipate at the same lock chain.</param>
        /// <returns></returns>
        public static async Task<LockResponse> RunAsync(this Func<Task> task, string lockId = "")
            => await Locks.Instance.RunAsync(task, lockId).NoContext();
    }
}
