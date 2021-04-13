using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem
{
    public static class Try
    {
        public static Catcher Execute(Func<Task> action) => new(action);
        public static Catcher<T> Catch<T>(Func<Task<T>> action) => new(action);
    }
    public class Catcher
    {
        private readonly Func<Task> Action;
        private readonly Dictionary<Type, Func<Exception, Task>> Catches = new();
        internal Catcher(Func<Task> action) => Action = action;
        public Catcher Catch<TException>(Func<Exception, Task> action)
            where TException : Exception
        {
            Catches.Add(typeof(TException), action);
            return this;
        }
        public async Task<CatchResponse> InvokeAsync()
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
                return new CatchResponse(ex);
            }
        }
    }
    public class Catcher<T>
    {
        private readonly Func<Task<T>> Action;
        internal Catcher(Func<Task<T>> action) => Action = action;
        private readonly Dictionary<Type, Func<Exception, Task>> Catches = new();
        public Catcher<T> Catch<TException>(Func<Exception, Task> action)
            where TException : Exception
        {
            Catches.Add(typeof(TException), action);
            return this;
        }
        public async Task<CatchResponse<T>> InvokeAsync()
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
                return new CatchResponse<T>(ex);
            }
        }
    }
    public class CatchResponse
    {
        public Exception Exception { get; }
        public bool InException => this.Exception != null;
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
    }
}