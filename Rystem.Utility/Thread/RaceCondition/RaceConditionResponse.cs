using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Concurrency
{
    public sealed class RaceConditionResponse
    {
        public bool IsExecuted { get; }
        public AggregateException Exceptions { get; }
        public bool InException => this.Exceptions != null;
        public RaceConditionResponse(bool isExecuted, IList<Exception> exceptions)
        {
            this.IsExecuted = isExecuted;
            if (exceptions != null && exceptions.Count > 0)
                this.Exceptions = new AggregateException(exceptions);
        }
    }
}
