using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.Identity
{
    internal partial class RystemIdentityStore : IUserLockoutStore<IdentityUser>
    {
        public Task<int> GetAccessFailedCountAsync(IdentityUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.AccessFailedCount);

        public Task<bool> GetLockoutEnabledAsync(IdentityUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.LockoutEnabled);

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(IdentityUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.LockoutEnd);

        public Task<int> IncrementAccessFailedCountAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            user.AccessFailedCount = 0;
            return Task.CompletedTask;
        }

        public Task SetLockoutEnabledAsync(IdentityUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.LockoutEnabled = enabled;
            return Task.CompletedTask;
        }

        public Task SetLockoutEndDateAsync(IdentityUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            user.LockoutEnd = lockoutEnd;
            return Task.CompletedTask;
        }
    }
}
