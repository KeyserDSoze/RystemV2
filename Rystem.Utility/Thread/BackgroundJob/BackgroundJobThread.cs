using Rystem.Concurrency;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.Background
{
    internal sealed class BackgroundJobThread
    {
        private static readonly Dictionary<string, System.Timers.Timer> Actions = new();
        public static Task AddTaskAsync(Func<Task> action, string id, Func<double> nextRunningTime = default, bool runImmediately = false, CancellationToken cancellationToken = default)
        {
            return Lock.RunAsync(async () =>
            {
                if (Actions.ContainsKey(id))
                {
                    Actions[id].Stop();
                    Actions.Remove(id);
                }
                if (runImmediately)
                    await action.Invoke().NoContext();
                NewTimer();

                void NewTimer()
                {
                    double value = Math.Ceiling(nextRunningTime?.Invoke() ?? 120);
                    bool runAction = true;
                    if (value > int.MaxValue)
                    {
                        runAction = false;
                        value = int.MaxValue;
                    }
                    var nextTimeTimer = new System.Timers.Timer
                    {
                        Interval = value
                    };
                    nextTimeTimer.Elapsed += async (x, e) =>
                    {
                        nextTimeTimer.Stop();
                        Actions.Remove(id);
                        if (!(cancellationToken != default && cancellationToken.IsCancellationRequested))
                        {
                            if (runAction)
                                await action.Invoke();
                            NewTimer();
                        }
                    };
                    nextTimeTimer.Start();
                    Actions.Add(id, nextTimeTimer);
                }
            }, $"{nameof(BackgroundJobOptions)}{id}");
        }
        public static Task AddTaskAsync(Action action, string id, Func<double> nextRunningTime = default, bool runImmediately = false, CancellationToken cancellationToken = default)
            => AddTaskAsync(() => { action(); return Task.CompletedTask; }, id, nextRunningTime, runImmediately, cancellationToken);
        public static Task RemoveTaskAsync(string id)
        {
            return Lock.RunAsync(() =>
            {
                if (Actions.ContainsKey(id))
                {
                    Actions[id].Stop();
                    Actions.Remove(id);
                }
                return Task.CompletedTask;
            }, $"{nameof(BackgroundJobOptions)}{id}");
        }
        public static bool IsRunning(string id)
            => Actions.ContainsKey(id);
    }
}