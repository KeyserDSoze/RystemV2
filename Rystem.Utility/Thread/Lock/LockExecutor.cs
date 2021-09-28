using System;
using System.Collections.Generic;
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
            try
            {
                await action.Invoke().NoContext();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            await implementation.ReleaseAsync(Key).NoContext();
            return new LockResponse(DateTime.UtcNow.Subtract(start), exception != default ? new List<Exception>() { exception } : null);
        }
    }
}