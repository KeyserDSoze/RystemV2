using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rystem.Business
{
    internal interface ICacheImplementation<T> : IWarmUp
    {
        Task<T> InstanceAsync(string key);
        Task<bool> UpdateAsync(string key, T value, TimeSpan expiringTime);
        Task<CacheStatus<T>> ExistsAsync(string key);
        Task<bool> DeleteAsync(string key);
        Task<IEnumerable<string>> ListAsync();
    }
}