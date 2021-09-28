using Rystem.Azure.Integration.Cache;
using System.Threading.Tasks;

namespace Rystem.Concurrency
{
    internal sealed class RedisCacheImplementation : IDistributedImplementation
    {
        private readonly RedisCacheIntegration Integration;
        public RedisCacheImplementation(RedisCacheIntegration integration) 
            => Integration = integration;

        public Task<bool> AcquireAsync(string key)
            => Integration.AcquireLockAsync(key);

        public Task<bool> IsAcquiredAsync(string key)
            => Integration.LockIsAcquiredAsync(key);
        public Task<bool> ReleaseAsync(string key)
            => Integration.ReleaseLockAsync(key);
        public Task<bool> WarmUpAsync()
            => Integration.WarmUpAsync();
    }
}