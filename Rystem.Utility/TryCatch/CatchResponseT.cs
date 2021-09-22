using System;

namespace Rystem
{
    public class CatchResponse<T> : CatchResponse
    {
        public T Result { get; }
        public CatchResponse(Exception exception) : base(exception) { }
        public CatchResponse(T result) : base(default)
            => this.Result = result;
        public new static CatchResponse<T> Empty { get; } = new CatchResponse<T>(default(T));
    }
}