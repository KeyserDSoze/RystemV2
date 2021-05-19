using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.BackgroundWork
{
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
        /// Method that allows task to run continuously in background.
        /// </summary>
        /// <param name="task">Action to perform.</param>
        /// <param name="id">Task id that runs in background.</param>
        /// <param name="runningTime">Time in milliseconds each time the task runs. Minimum time is 120ms.</param>
        public static void Run(Func<Task> task, string id = "", int runningTime = 120)
            => task.RunInBackground(id, runningTime);
        /// <summary>
        /// Remove a task from the continuously running by its id.
        /// </summary>
        /// <param name="id">Task id that runs in background.</param>
        public static void Stop(string id = "")
            => GhostdExtensions.StopRunningInBackground(null, id);
    }
    public static class GhostdExtensions
    {
        /// <summary>
        /// Method that allows task to run continuously in background.
        /// </summary>
        /// <param name="task">Action to perform</param>
        /// <param name="id">Task id that runs in background.</param>
        /// <param name="runningTime">Time in milliseconds each time the task runs. Minimum time is 120ms.</param>
        public static void RunInBackground(this Action task, string id = "", int runningTime = 120)
            => GhostThread.AddTask(task, id, runningTime);
        public static void RunInBackground(this Func<Task> task, string id = "", int runningTime = 120)
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