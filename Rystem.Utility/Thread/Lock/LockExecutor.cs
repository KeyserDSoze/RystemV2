using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Concurrency
{
    internal sealed class LockExecutor
    {
        private DateTime LastExecutionPlusExpirationTime;
        internal bool IsExpired => DateTime.UtcNow > LastExecutionPlusExpirationTime;
        private readonly MemoryImplementation Memory = new();
        private readonly string Key;
        public LockExecutor(string key)
            => Key = key;
        public async Task<LockResponse> ExecuteAsync(Func<Task> action, IDistributedImplementation implementation)
        {
            implementation ??= Memory;
            DateTime start = DateTime.UtcNow;
            LastExecutionPlusExpirationTime = start.AddDays(1);
            while (true)
            {
                if (await implementation.AcquireAsync(Key).NoContext())
                    break;
                await Task.Delay(2).NoContext();
            }
            Exception exception = default;
            var result = await Try.Execute(action).InvokeAsync().NoContext();
            if (result.InException)
                exception = result.Exception;
            await implementation.ReleaseAsync(Key).NoContext();
            return new LockResponse(DateTime.UtcNow.Subtract(start), exception != default ? new List<Exception>() { exception } : null);
        }
    }
}