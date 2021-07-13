using System;
using System.Threading.Tasks;

namespace Rystem.Background
{
    public static class BackgroundWork
    {
        /// <summary>
        /// Method that allows task to run continuously in background.
        /// </summary>
        /// <param name="task">Action to perform.</param>
        /// <param name="nextRunningTime">Time in milliseconds each time the task runs after the first. If firstRunningTime has a value, the first running time is used instead. Default time is 120ms.</param>
        /// <param name="firstRunningTime">Time in milliseconds for first time the task runs.</param>
        public static void Run(Action task, string id = "", Func<int> nextRunningTime = default, bool runImmediately = false)
            => task.RunInBackground(id, nextRunningTime, runImmediately);
        /// <summary>
        /// Method that allows task to run continuously in background.
        /// </summary>
        /// <param name="task">Action to perform.</param>
        /// <param name="id">Task id that runs in background.</param>
        /// <param name="nextRunningTime">Time in milliseconds each time the task runs after the first. If firstRunningTime has a value, the first running time is used instead. Default time is 120ms.</param>
        /// <param name="firstRunningTime">Time in milliseconds for first time the task runs.</param>
        public static void Run(Func<Task> task, string id = "", Func<int> nextRunningTime = default, bool runImmediately = false)
            => task.RunInBackground(id, nextRunningTime, runImmediately);
        /// <summary>
        /// Remove a task from the continuously running by its id.
        /// </summary>
        /// <param name="id">Task id that runs in background.</param>
        public static void Stop(string id = "")
            => BackgroundWorkExtensions.StopRunningInBackground(null, id);
    }
    public static partial class BackgroundWorkExtensions
    {
        /// <summary>
        /// Method that allows task to run continuously in background.
        /// </summary>
        /// <param name="task">Action to perform</param>
        /// <param name="id">Task id that runs in background.</param>
        /// <param name="nextRunningTime">Time in milliseconds each time the task runs after the first. If firstRunningTime has a value, the first running time is used instead.  Default time is 120ms.</param>
        /// <param name="firstRunningTime">Time in milliseconds for first time the task runs.</param>
        public static void RunInBackground(this Action task, string id = "", Func<int> nextRunningTime = default, bool runImmediately = false)
            => BackgroundWorkThread.AddTask(task, id, nextRunningTime, runImmediately);
        /// <summary>
        /// Method that allows task to run continuously in background.
        /// </summary>
        /// <param name="task">Action to perform</param>
        /// <param name="id">Task id that runs in background.</param>
        /// <param name="nextRunningTime">Time in milliseconds each time the task runs after the first. If firstRunningTime has a value, the first running time is used instead.  Default time is 120ms.</param>
        /// <param name="firstRunningTime">Time in milliseconds for first time the task runs.</param>
        public static void RunInBackground(this Func<Task> task, string id = "", Func<int> nextRunningTime = default, bool runImmediately = false)
            => BackgroundWorkThread.AddTask(task, id, nextRunningTime, runImmediately);
        /// <summary>
        /// Remove a task from the continuously running by its id.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="id">Task id that runs in background.</param>
        public static void StopRunningInBackground(this Action task, string id = "")
            => BackgroundWorkThread.RemoveTask(id);
    }
}