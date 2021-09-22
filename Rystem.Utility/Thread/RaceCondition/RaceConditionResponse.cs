using System;
using System.Collections.Generic;

namespace Rystem.Concurrency
{
    public sealed class RaceConditionResponse
    {
        public bool IsExecuted { get; }
        public AggregateException Exceptions { get; }
        public bool InException => this.Exceptions != default;
        public RaceConditionResponse(bool isExecuted, IList<Exception> exceptions)
        {
            this.IsExecuted = isExecuted;
            if (exceptions != default && exceptions.Count > 0)
                this.Exceptions = new AggregateException(exceptions);
        }
    }
}
