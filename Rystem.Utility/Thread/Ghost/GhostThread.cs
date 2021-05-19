﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.BackgroundWork
{
    internal sealed class GhostThread
    {
        private static readonly Dictionary<string, System.Timers.Timer> Actions = new();
        private static readonly object Semaphore = new();
        public static void AddTask(Action action, string id, int milliseconds, CancellationToken cancellationToken = default)
        {
            lock (Semaphore)
            {
                if (Actions.ContainsKey(id))
                {
                    Actions[id].Stop();
                    Actions.Remove(id);
                }
                var performanceTimer = new System.Timers.Timer
                {
                    Interval = milliseconds
                };
                performanceTimer.Elapsed += (x, e) =>
                {
                    if (cancellationToken != default && cancellationToken.IsCancellationRequested)
                        performanceTimer.Stop();
                    action.Invoke();
                };
                performanceTimer.Start();
                Actions.Add(id, performanceTimer);
            }
        }
        public static void AddTask(Func<Task> action, string id, int milliseconds, CancellationToken cancellationToken = default)
        {
            AddTask(() => { action(); return; }, id, milliseconds, cancellationToken);
        }
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
    }
}