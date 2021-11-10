using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.Business.Identity
{
    internal partial class RystemIdentityStore : IUserTwoFactorStore<IdentityUser>
    {
        public Task<bool> GetTwoFactorEnabledAsync(IdentityUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.TwoFactorEnabled);

        public Task SetTwoFactorEnabledAsync(IdentityUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.TwoFactorEnabled = enabled;
            return Task.CompletedTask;
        }
    }
}