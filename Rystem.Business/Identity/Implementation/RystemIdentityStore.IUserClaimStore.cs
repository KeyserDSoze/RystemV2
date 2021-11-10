using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.Business.Identity
{
    internal partial class RystemIdentityStore : IUserClaimStore<IdentityUser>
    {
        public Task AddClaimsAsync(IdentityUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
            => ClaimForAccountStoreManager.UpdateBatchAsync(
                claims.Select(x => new ClaimsForAccount { Id = user.Id, ClaimId = x.Type, ClaimValue = x.Value }));

        public async Task<IList<Claim>> GetClaimsAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            var claims = (await ClaimForAccountStoreManager.GetAsync(default, x => x.Id == user.Id).NoContext())
                .Select(x => new Claim(x.ClaimId, x.ClaimValue))
                .ToList();
            return claims;
        }
        public async Task<IList<IdentityUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            var claims = await ClaimForAccountStoreManager.GetAsync(default, x => x.ClaimId == claim.Type && x.ClaimValue == claim.Value).NoContext();
            List<IdentityUser> users = new();
            foreach (var user in claims)
                users.Add(await IdentityStoreManager.FirstOrDefaultAsync(default, x => x.Id == user.Id).NoContext());
            return users;
        }
        public Task RemoveClaimsAsync(IdentityUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
            => ClaimForAccountStoreManager.DeleteBatchAsync(
                claims.Select(x => new ClaimsForAccount { Id = user.Id, ClaimId = x.Type, ClaimValue = x.Value }));
        public async Task ReplaceClaimAsync(IdentityUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            await ClaimForAccountStoreManager.UpdateAsync(new ClaimsForAccount() { Id = user.Id, ClaimId = newClaim.Type, ClaimValue = newClaim.Value }).NoContext();
            if (claim.Type != newClaim.Type)
                await ClaimForAccountStoreManager.DeleteAsync(new ClaimsForAccount() { Id = user.Id, ClaimId = claim.Type }).NoContext();
        }
    }
}