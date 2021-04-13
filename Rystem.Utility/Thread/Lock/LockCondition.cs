using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Utility.Thread
{
    internal sealed class LockCondition
    {
        private readonly object Semaphore = new();
        private DateTime LastExecutionPlusExpirationTime;
        internal bool IsExpired => DateTime.UtcNow > LastExecutionPlusExpirationTime;
        private bool IsLocked { get; set; }
        public async Task<LockConditionResponse> ExecuteAsync(Func<Task> action)
        {
            DateTime start = DateTime.UtcNow;
            LastExecutionPlusExpirationTime = start.AddDays(1);
            while (!Lock())
                await Task.Delay(2).NoContext();
            Exception exception = default;
            var result = await Try.Execute(action).InvokeAsync().NoContext();
            if (result.InException)
                exception = result.Exception;
            this.IsLocked = false;
            return new LockConditionResponse(DateTime.UtcNow.Subtract(start), exception != default ? new List<Exception>() { exception } : null);

            bool Lock()
            {
                if (!IsLocked)
                    lock (Semaphore)
                        if (!IsLocked)
                        {
                            IsLocked = true;
                            return true;
                        }
                return false;
            }
        }
    }
}
