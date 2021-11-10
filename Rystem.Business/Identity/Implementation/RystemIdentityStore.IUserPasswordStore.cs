using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.Business.Identity
{
    internal partial class RystemIdentityStore : IUserPasswordStore<IdentityUser>
    {
        public Task<string> GetPasswordHashAsync(IdentityUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.PasswordHash);

        public Task<bool> HasPasswordAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            if (user.Id.Split(Separator).Length > 1)
                return Task.FromResult(false);
            else
                return Task.FromResult(true);
        }

        public Task SetPasswordHashAsync(IdentityUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }
    }
}
