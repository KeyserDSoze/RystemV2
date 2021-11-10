using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.Business.Identity
{
    internal partial class RystemIdentityStore : IUserEmailStore<IdentityUser>
    {
        public Task<IdentityUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
            => FindByNameAsync(normalizedEmail, cancellationToken);

        public Task<string> GetEmailAsync(IdentityUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.Email);

        public Task<bool> GetEmailConfirmedAsync(IdentityUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.EmailConfirmed);

        public Task<string> GetNormalizedEmailAsync(IdentityUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.NormalizedEmail);

        public Task SetEmailAsync(IdentityUser user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task SetEmailConfirmedAsync(IdentityUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task SetNormalizedEmailAsync(IdentityUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }
    }
}
