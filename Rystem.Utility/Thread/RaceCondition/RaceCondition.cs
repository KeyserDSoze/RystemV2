using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Utility.Thread
{
    internal sealed class RaceCondition
    {
        private readonly object Semaphore = new();
        private DateTime LastExecutionPlusExpirationTime;
        internal bool IsExpired => DateTime.UtcNow > LastExecutionPlusExpirationTime;
        private bool IsLocked { get; set; }
        public async Task<RaceConditionResponse> ExecuteAsync(Func<Task> action)
        {
            LastExecutionPlusExpirationTime = DateTime.UtcNow.AddDays(1);
            var isTheFirst = false;
            var isWaiting = false;
            await WaitAsync().NoContext();
            if (!isWaiting)
            {
                lock (Semaphore)
                    if (!IsLocked)
                    {
                        IsLocked = true;
                        isTheFirst = true;
                    }
                if (!isTheFirst)
                    await WaitAsync().NoContext();
            }
            Exception exception = default;
            if (isTheFirst && !isWaiting)
            {
                var result = await Try.Execute(action).InvokeAsync().NoContext();
                if (result.InException)
                    exception = result.Exception;
                this.IsLocked = false;
            }
            return new RaceConditionResponse(isTheFirst && !isWaiting, exception != default ? new List<Exception>() { exception } : null);

            async Task WaitAsync()
            {
                while (IsLocked)
                {
                    isWaiting = true;
                    await Task.Delay(4).NoContext();
                }
            }
        }
    }
}
