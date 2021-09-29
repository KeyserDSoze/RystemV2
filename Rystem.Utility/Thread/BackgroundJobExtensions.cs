using System;
using System.Threading.Tasks;

namespace Rystem.Background
{
    public static partial class BackgroundJobExtensions
    {
        /// <summary>
        /// Method that allows task to run continuously in background.
        /// </summary>
        /// <param name="task">Action to perform</param>
        /// <param name="id">Task id that runs in background.</param>
        /// <param name="nextRunningTime">Function that has to return a value in milliseconds. Default time is 120ms.</param>
        /// <param name="runImmediately">Run immediately at the start.</param>
        public static Task RunInBackgroundAsync(this Action task, string id = "", Func<double> nextRunningTime = default, bool runImmediately = false)
            => BackgroundJobThread.AddTaskAsync(task, id, nextRunningTime, runImmediately);
        /// <summary>
        /// Method that allows task to run continuously in background.
        /// </summary>
        /// <param name="task">Action to perform</param>
        /// <param name="id">Task id that runs in background.</param>
        /// <param name="nextRunningTime">Function that has to return a value in milliseconds. Default time is 120ms.</param>
        /// <param name="runImmediately">Run immediately at the start.</param>
        public static Task RunInBackgroundAsync(this Func<Task> task, string id = "", Func<double> nextRunningTime = default, bool runImmediately = false)
            => BackgroundJobThread.AddTaskAsync(task, id, nextRunningTime, runImmediately);
        /// <summary>
        /// Remove a task from the continuously running by its id.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="id">Task id that runs in background.</param>
        public static Task StopRunningInBackgroundAsync(this Action task, string id = "")
            => BackgroundJobThread.RemoveTaskAsync(id);
        /// <summary>
        /// Get if your task is running.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="id">Task id that runs in background.</param>
        public static bool IsRunning(this Action task, string id = "")
            => BackgroundJobThread.IsRunning(id);
    }
}