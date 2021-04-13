using Rystem.Utility;
using Rystem.Utility.Thread;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class Lock
    {
        /// <summary>
        /// Deal with concurrency and allow only one method to run. Other concurrent task will be dropped.
        /// </summary>
        /// <param name="task">Action to perform</param>
        /// <param name="lockId">lock key, task with different id doesn't partecipate at the same lock chain.</param>
        /// <returns></returns>
        public static async Task<LockConditionResponse> RunAsync(this Func<Task> task, string lockId = "")
            => await LockConditionInstances.Instance.RunAsync(task, lockId).NoContext();
    }
    public static class RaceCondition
    {
        /// <summary>
        /// Deal with concurrency and allow only one method to run. Other concurrent task will be dropped.
        /// </summary>
        /// <param name="task">Action to perform</param>
        /// <param name="raceId">concurrency key, task with different id doesn't partecipate at the same race.</param>
        /// <returns></returns>
        public static async Task<RaceConditionResponse> RunAsync(Func<Task> task, string raceId = "")
            => await task.RunUnderRaceConditionAsync(raceId).NoContext();
    }
    public static class Ghost
    {
        /// <summary>
        /// Method that allows task to run continuously in background.
        /// </summary>
        /// <param name="task">Action to perform.</param>
        /// <param name="id">Task id that runs in background.</param>
        /// <param name="runningTime">Time in milliseconds each time the task runs. Minimum time is 120ms.</param>
        public static void Run(Action task, string id = "", int runningTime = 120)
            => task.RunInBackground(id, runningTime);
        /// <summary>
        /// Remove a task from the continuously running by its id.
        /// </summary>
        /// <param name="id">Task id that runs in background.</param>
        public static void Stop(string id = "")
            => ThreadExtensions.StopRunningInBackground(null, id);
    }
    public static class ThreadExtensions
    {
        /// <summary>
        /// Deal with concurrency and allow only one method to run. Other concurrent task will be dropped.
        /// </summary>
        /// <param name="task">Action to perform</param>
        /// <param name="lockId">lock key, task with different id doesn't partecipate at the same lock chain.</param>
        /// <returns></returns>
        public static async Task<LockConditionResponse> LockAsync(this Func<Task> task, string lockId = "")
            => await LockConditionInstances.Instance.RunAsync(task, lockId).NoContext();
        /// <summary>
        /// Deal with concurrency and allow only one method to run. Other concurrent task will be dropped.
        /// </summary>
        /// <param name="task">Action to perform</param>
        /// <param name="raceId">concurrency key, task with different id doesn't partecipate at the same race.</param>
        /// <returns></returns>
        public static async Task<RaceConditionResponse> RunUnderRaceConditionAsync(this Func<Task> task, string raceId = "")
            => await RaceConditionInstances.Instance.RunAsync(task, raceId).NoContext();
        /// <summary>
        /// Method that allows task to run continuously in background.
        /// </summary>
        /// <param name="task">Action to perform</param>
        /// <param name="id">Task id that runs in background.</param>
        /// <param name="runningTime">Time in milliseconds each time the task runs. Minimum time is 120ms.</param>
        public static void RunInBackground(this Action task, string id = "", int runningTime = 120)
            => GhostThread.AddTask(task, id, runningTime);
        /// <summary>
        /// Remove a task from the continuously running by its id.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="id">Task id that runs in background.</param>
        public static void StopRunningInBackground(this Action task, string id = "")
            => GhostThread.RemoveTask(id);
    }
}
