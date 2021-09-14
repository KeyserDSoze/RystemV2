using System;
using System.Threading.Tasks;

namespace Rystem.Cache
{
    public interface ICacheService
    {
        Task<bool> ExistsAsync(string key);
        Task<HttpResponseCache> InstanceAsync(string key);
        Task UpdateAsync(string key, HttpResponseCache entity, TimeSpan expireAfter);
    }
}