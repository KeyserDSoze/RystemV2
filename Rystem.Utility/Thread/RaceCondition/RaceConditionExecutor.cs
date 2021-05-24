using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Concurrency
{
    internal sealed class RaceConditionExecutor
    {
        private DateTime LastExecutionPlusExpirationTime;
        internal bool IsExpired => DateTime.UtcNow > LastExecutionPlusExpirationTime;
        private readonly string Key;
        public RaceConditionExecutor(string id)
            => Key = id;
        private readonly MemoryImplementation Memory = new();
        public async Task<RaceConditionResponse> ExecuteAsync(Func<Task> action, IDistributedImplementation implementation)
        {
            implementation ??= Memory;
            LastExecutionPlusExpirationTime = DateTime.UtcNow.AddDays(1);
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
                var result = await Try.Execute(action).InvokeAsync().NoContext();
                if (result.InException)
                    exception = result.Exception;
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
