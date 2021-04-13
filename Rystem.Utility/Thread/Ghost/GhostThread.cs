﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.Utility
{
    internal class GhostThread
    {
        private static readonly Dictionary<int, System.Timers.Timer> Actions = new();
        private static readonly object Semaphore = new();
        public static void AddTask(Action action, int id, int milliseconds)
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
                performanceTimer.Elapsed += (x, e) => action.Invoke();
                performanceTimer.Start();
                Actions.Add(id, performanceTimer);
            }
        }
        public static void RemoveTask(int id)
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