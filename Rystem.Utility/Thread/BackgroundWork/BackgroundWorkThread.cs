using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.Background
{
    internal sealed class BackgroundWorkThread
    {
        private static readonly Dictionary<string, System.Timers.Timer> Actions = new();
        private static readonly object Semaphore = new();
        public static void AddTask(Func<Task> action, string id, Func<double> nextRunningTime = default, bool runImmediately = false, CancellationToken cancellationToken = default)
        {
            lock (Semaphore)
            {
                if (Actions.ContainsKey(id))
                {
                    Actions[id].Stop();
                    Actions.Remove(id);
                }
                if (runImmediately)
                    action.Invoke().NoContext();
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
            }
        }
        public static void AddTask(Action action, string id, Func<double> nextRunningTime = default, bool runImmediately = false, CancellationToken cancellationToken = default)
            => AddTask(() => { action(); return Task.CompletedTask; }, id, nextRunningTime, runImmediately, cancellationToken);
        public static void RemoveTask(string id)
        {
            lock (Semaphore)
            {
                if (Actions.ContainsKey(id))
                {
                    Actions[id].Stop();
                    Actions.Remove(id);
                }
            }
        }
        public static bool IsRunning(string id)
            => Actions.ContainsKey(id);
    }
}