using System;
using System.Collections.Generic;

namespace Rystem.Business
{
    public interface IQueue
    {
        RystemQueueServiceProvider ConfigureQueue();
        internal RystemQueueServiceProvider BuildQueue()
            => ConfigureQueue().AddInstance(this.GetType());
    }
}