using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.Identity
{
    internal partial class RystemIdentityStore : IUserSecurityStampStore<IdentityUser>
    {
        public Task<string> GetSecurityStampAsync(IdentityUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.SecurityStamp);

        public Task SetSecurityStampAsync(IdentityUser user, string stamp, CancellationToken cancellationToken)
        {
            user.SecurityStamp = stamp;
            return Task.CompletedTask;
        }
    }
}
