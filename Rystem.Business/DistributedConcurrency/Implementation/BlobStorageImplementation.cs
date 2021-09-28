using Rystem.Azure.Integration.Storage;
using System.Threading.Tasks;

namespace Rystem.Concurrency
{
    internal sealed class BlobStorageImplementation : IDistributedImplementation
    {
        private readonly BlobStorageIntegration Integration;
        internal BlobStorageImplementation(BlobStorageIntegration integration)
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