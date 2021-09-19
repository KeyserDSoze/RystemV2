using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rystem.Business
{
    public interface ICacheManager<TCacheKey, TCache>
        where TCacheKey : ICacheKey<TCache>
    {
        Task<TCache> InstanceAsync(TCacheKey key, bool withConsistency, TimeSpan expiringTime, Installation installation = Installation.Default);
        Task<bool> UpdateAsync(TCacheKey key, TCache value, TimeSpan expiringTime, Installation installation = Installation.Default);
        Task<bool> ExistsAsync(TCacheKey key, Installation installation = Installation.Default);
        Task<bool> DeleteAsync(TCacheKey key, Installation installation = Installation.Default);
        Task<IEnumerable<string>> ListAsync(Installation installation = Installation.Default);
    }
}