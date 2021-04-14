using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// <returns></returns>
        public static async Task<RaceConditionResponse> RunAsync(Func<Task> task, string raceId = "")
            => await task.RunUnderRaceConditionAsync(raceId).NoContext();
    }
}
