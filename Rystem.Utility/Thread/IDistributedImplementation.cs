using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rystem.Concurrency
{
    public interface IDistributedImplementation
    {
        Task<bool> AcquireAsync(string key);
        Task<bool> IsAcquiredAsync(string key);
        Task<bool> ReleaseAsync(string key);
    }
}