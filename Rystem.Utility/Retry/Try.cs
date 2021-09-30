using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System
{
    public static class Try
    {
        public static async Task<TryResponse<T>> ExecuteAsync<T>(this Func<Task<T>> action, int times = 5, TimeSpan awaitTime = default)
        {
            int time = 0;
            var exceptions = new List<Exception>();
            while (time < times)
            {
                try
                {
                    return new TryResponse<T>(await action.Invoke().NoContext(), exceptions, true, exceptions.Select(x => x.Message).Distinct().Count());
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    time++;
                    if (awaitTime != default)
                        await Task.Delay((int)awaitTime.TotalMilliseconds);
                }
            }
            return new TryResponse<T>(default, exceptions, false, exceptions.Select(x => x.Message).Distinct().Count());
        }
    }
}