using System;
using System.Threading.Tasks;

namespace Rystem.Background
{
    public static class BackgroundJob
    {
        /// <summary>
        /// Method that allows task to run continuously in background.
        /// </summary>
        /// <param name="task">Action to perform.</param>
        /// <param name="nextRunningTime">Function that has to return a value in milliseconds. Default time is 120ms.</param>
        /// <param name="runImmediately">Run immediately at the start.</param>
        public static void Run(Action task, string id = "", Func<double> nextRunningTime = default, bool runImmediately = false)
            => task.RunInBackground(id, nextRunningTime, runImmediately);
        /// <summary>
        /// Method that allows task to run continuously in background.
        /// </summary>
        /// <param name="task">Action to perform.</param>
        /// <param name="id">Task id that runs in background.</param>
        /// <param name="nextRunningTime">Function that has to return a value in milliseconds. Default time is 120ms.</param>
        /// <param name="runImmediately">Run immediately at the start.</param>
        public static void Run(Func<Task> task, string id = "", Func<double> nextRunningTime = default, bool runImmediately = false)
            => task.RunInBackground(id, nextRunningTime, runImmediately);
        /// <summary>
        /// Remove a task from the continuously running by its id.
        /// </summary>
        /// <param name="id">Task id that runs in background.</param>
        public static void Stop(string id = "")
            => BackgroundJobExtensions.StopRunningInBackground(null, id);
        /// <summary>
        /// Get if your task is running.
        /// </summary>
        /// <param name="id">Task id that runs in background.</param>
        public static bool IsRunning(string id = "")
            => BackgroundJobExtensions.IsRunning(null, id);
    }
}