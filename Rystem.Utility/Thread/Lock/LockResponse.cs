﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Concurrency
{
    public sealed class LockResponse
    {
        public TimeSpan ExecutionTime { get; }
        public AggregateException Exceptions { get; }
        public bool InException => this.Exceptions != default;
        public LockResponse(TimeSpan executionTime, IList<Exception> exceptions)
        {
            this.ExecutionTime = executionTime;
            if (exceptions != default && exceptions.Count > 0)
                this.Exceptions = new AggregateException(exceptions);
        }
    }
}
