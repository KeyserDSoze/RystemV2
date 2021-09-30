using System.Threading.Tasks;

namespace System
{
    public static class Stopwatch
    {
        public static StopwatchStart Start() => new();
        public static async Task<StopwatchResult> ExecuteAsync(this Func<Task> action)
        {
            var start = Start();
            await action.Invoke().NoContext();
            return start.Stop();
        }
    }
}