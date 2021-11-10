using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.Business.Identity
{
    internal partial class RystemIdentityStore : IUserPhoneNumberStore<IdentityUser>
    {
        public Task<string> GetPhoneNumberAsync(IdentityUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.PhoneNumber);

        public Task<bool> GetPhoneNumberConfirmedAsync(IdentityUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.PhoneNumberConfirmed);

        public Task SetPhoneNumberAsync(IdentityUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            user.PhoneNumber = phoneNumber;
            return Task.CompletedTask;
        }

        public Task SetPhoneNumberConfirmedAsync(IdentityUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.PhoneNumberConfirmed = confirmed;
            return Task.CompletedTask;
        }
    }
}
