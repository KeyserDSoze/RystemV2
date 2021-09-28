using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rystem.Concurrency
{
    internal sealed class RaceConditionExecutor
    {
        private DateTime LastExecutionPlusExpirationTime;
        internal bool IsExpired => DateTime.UtcNow > LastExecutionPlusExpirationTime;
        private readonly string Key;
        private readonly TimeSpan TimeWindow;
        public RaceConditionExecutor(string id, TimeSpan timeWindow)
        {
            Key = id;
            TimeWindow = timeWindow == default ? TimeSpan.FromMinutes(1) : timeWindow;
        }
        private readonly MemoryImplementation Memory = new();
        public async Task<RaceConditionResponse> ExecuteAsync(Func<Task> action, IDistributedImplementation implementation)
        {
            implementation ??= Memory;
            LastExecutionPlusExpirationTime = DateTime.UtcNow.Add(TimeWindow);
            var isTheFirst = false;
            var isWaiting = false;
            await WaitAsync().NoContext();
            if (!isWaiting)
            {
                if (await implementation.AcquireAsync(Key).NoContext())
                    isTheFirst = true;
                if (!isTheFirst)
                    await WaitAsync().NoContext();
            }
            Exception exception = default;
            if (isTheFirst && !isWaiting)
            {
                try
                {
                    await action.Invoke().NoContext();
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
                await implementation.ReleaseAsync(Key).NoContext();
            }
            return new RaceConditionResponse(isTheFirst && !isWaiting, exception != default ? new List<Exception>() { exception } : null);

            async Task WaitAsync()
            {
                while (await implementation.IsAcquiredAsync(Key).NoContext())
                {
                    isWaiting = true;
                    await Task.Delay(4).NoContext();
                }
            }
        }
    }
}
