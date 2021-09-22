using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rystem
{
    public class Catcher
    {
        private readonly Func<Task> Action;
        private readonly Dictionary<Type, Func<Exception, Task>> Catches = new();
        private readonly int RunningAttempts;
        internal Catcher(Func<Task> action, int runninAttempts)
        {
            Action = action;
            RunningAttempts = runninAttempts;
        }
        public Catcher Catch<TException>(Func<Exception, Task> action)
            where TException : Exception
        {
            Catches.Add(typeof(TException), action);
            return this;
        }
        public async Task<CatchResponse> InvokeAsync()
        {
            CatchResponse catchResponse = CatchResponse.Empty;
            for (int i = 0; i < RunningAttempts; i++)
            {
                try
                {
                    await Action.Invoke().NoContext();
                    return CatchResponse.Empty;
                }
                catch (Exception ex)
                {
                    Type type = ex.GetType();
                    if (Catches.ContainsKey(type))
                        await Catches[type].Invoke(ex);
                    catchResponse = new CatchResponse(ex);
                }
            }
            return catchResponse;
        }
    }
}