using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem
{
    public static class Try
    {
        public static Catcher Execute(Func<Task> action, int runninAttempts = 1) => new(action, runninAttempts);
        public static Catcher<T> Execute<T>(Func<Task<T>> action, int runninAttempts = 1) => new(action, runninAttempts);
    }
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
    public class CatchResponse
    {
        public Exception Exception { get; }
        public bool InException => this.Exception != default;
        public CatchResponse(Exception exception)
            => this.Exception = exception;
        private CatchResponse() { }
        public static CatchResponse Empty { get; } = new CatchResponse();
    }
    public class CatchResponse<T> : CatchResponse
    {
        public T Result { get; }
        public CatchResponse(Exception exception) : base(exception) { }
        public CatchResponse(T result) : base(default)
            => this.Result = result;
        public new static CatchResponse<T> Empty { get; } = new CatchResponse<T>(default(T));
    }
}