using System;

namespace Rystem
{
    public class CatchResponse
    {
        public Exception Exception { get; }
        public bool InException => this.Exception != default;
        public CatchResponse(Exception exception)
            => this.Exception = exception;
        private CatchResponse() { }
        public static CatchResponse Empty { get; } = new CatchResponse();
    }
}