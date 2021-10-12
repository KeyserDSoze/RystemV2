using Polly;
using Polly.CircuitBreaker;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System
{
    public static class Try
    {
        public static async Task<TryResponse<T, Exception>> ExecuteAsync<T>(this Func<Task<T>> action, int times = 5, TimeSpan durantionOfBreak = default, Func<Exception, int, Task> onException = default)
        {
            List<Exception> exceptions = new();
            var execution = await Policy
                .Handle<Exception>()
                .RetryAsync(times, onRetry: async (exception, retryCount) =>
                {
                    if (onException != default)
                        await onException(exception, retryCount).NoContext();
                    if (durantionOfBreak != default)
                        await Task.Delay((int)durantionOfBreak.TotalMilliseconds);
                    exceptions.Add(exception);
                })
                .ExecuteAndCaptureAsync(action).NoContext();
            return new(execution.Result, exceptions, execution.Outcome == OutcomeType.Successful, exceptions.Select(x => x.Message).Distinct().Count());
        }
        public static TryResponse<T, Exception> Execute<T>(this Func<T> action, int times = 5, TimeSpan durantionOfBreak = default, Func<Exception, int, Task> onException = default)
        {
            List<Exception> exceptions = new();
            var execution = Policy
                .Handle<Exception>()
                .Retry(times, onRetry: async (exception, retryCount) =>
                {
                    if (onException != default)
                        await onException(exception, retryCount).NoContext();
                    if (durantionOfBreak != default)
                        await Task.Delay((int)durantionOfBreak.TotalMilliseconds);
                    exceptions.Add(exception);
                })
                .ExecuteAndCapture(action);
            return new(execution.Result, exceptions, execution.Outcome == OutcomeType.Successful, exceptions.Select(x => x.Message).Distinct().Count());
        }
        public static async Task<TryResponse<T, TException>> ExecuteCircuitBreakerAsync<T, TException>(this Func<Task<T>> action, double failureThreshold = 0.5, TimeSpan samplingDuration = default, int minimumThroughput = 8, TimeSpan durationOfBreak = default, Func<TException, CircuitState, TimeSpan, Context, Task> onBreak = default, Func<Context, Task> onReset = default, Func<Task> onHalfOpen = default)
            where TException : Exception
        {
            if (samplingDuration == default)
                samplingDuration = TimeSpan.FromSeconds(20);
            if (durationOfBreak == default)
                durationOfBreak = TimeSpan.FromSeconds(20);
            List<TException> exceptions = new();
            var execution = await Policy
                .Handle<TException>()
                .AdvancedCircuitBreakerAsync(failureThreshold, samplingDuration, minimumThroughput, durationOfBreak,
                    async (exception, state, span, context) =>
                    {
                        if (exception is TException specifiedException)
                        {
                            if (onBreak != default)
                                await onBreak.Invoke(specifiedException, state, span, context).NoContext();
                            exceptions.Add(specifiedException);
                        }
                    },
                    async (context) =>
                    {
                        if (onReset != default)
                            await onReset.Invoke(context).NoContext();
                    },
                    async () =>
                    {
                        if (onHalfOpen != default)
                            await onHalfOpen.Invoke().NoContext();
                    })
                .ExecuteAndCaptureAsync(action).NoContext();
            return new(execution.Result, exceptions, execution.Outcome == OutcomeType.Successful, exceptions.Select(x => x.Message).Distinct().Count());
        }
    }
}