using Rystem.Azure.Integration.Storage;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.Concurrency
{
    internal class BlobStorageImplementation : IDistributedImplementation
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
    }
}