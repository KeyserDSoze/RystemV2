using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Utility.Thread
{
    public sealed class LockConditionResponse
    {
        public TimeSpan ExecutionTime { get; }
        public AggregateException Exceptions { get; }
        public bool InException => this.Exceptions != null;
        public LockConditionResponse(TimeSpan executionTime, IList<Exception> exceptions)
        {
            this.ExecutionTime = executionTime;
            if (exceptions != null && exceptions.Count > 0)
                this.Exceptions = new AggregateException(exceptions);
        }
    }
}
