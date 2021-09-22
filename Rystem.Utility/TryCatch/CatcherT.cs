using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rystem
{
    public class Catcher<T>
    {
        private readonly Func<Task<T>> Action;
        private readonly int RunningAttempts;
        internal Catcher(Func<Task<T>> action, int runningAttempts)
        {
            Action = action;
            RunningAttempts = runningAttempts;
        }
        private readonly Dictionary<Type, Func<Exception, Task>> Catches = new();
        public Catcher<T> Catch<TException>(Func<Exception, Task> action)
            where TException : Exception
        {
            Catches.Add(typeof(TException), action);
            return this;
        }
        public async Task<CatchResponse<T>> InvokeAsync()
        {
            CatchResponse<T> catchResponse = CatchResponse<T>.Empty;
            for (int i = 0; i < RunningAttempts; i++)
            {
                try
                {
                    return new CatchResponse<T>(await Action.Invoke().NoContext());
                }
                catch (Exception ex)
                {
                    Type type = ex.GetType();
                    if (Catches.ContainsKey(type))
                        await Catches[type].Invoke(ex);
                    catchResponse = new CatchResponse<T>(ex);
                }
            }
            return catchResponse;
        }
    }
}