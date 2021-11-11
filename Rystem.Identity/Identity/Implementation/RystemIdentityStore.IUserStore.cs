using Microsoft.AspNetCore.Identity;
using Rystem.Business.Document;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.Identity
{
    internal partial class RystemIdentityStore : IUserStore<IdentityUser>
    {
        private readonly IDocumentManager<IdentityUser> IdentityStoreManager;
        private readonly IDocumentManager<RolesForAccount> RoleForAccountStoreManager;
        private readonly IDocumentManager<ClaimsForAccount> ClaimForAccountStoreManager;
        private readonly IIdentityNotifications<IdentityUser, IdentityRole> Notificator;
        public RystemIdentityStore(
            IDocumentManager<IdentityUser> identityStoreManager,
            IDocumentManager<RolesForAccount> roleForAccountStoreManager,
            IDocumentManager<ClaimsForAccount> claimForAccountStoreManager)
        {
            IdentityStoreManager = identityStoreManager;
            RoleForAccountStoreManager = roleForAccountStoreManager;
            ClaimForAccountStoreManager = claimForAccountStoreManager;
        }
        public RystemIdentityStore(
            IDocumentManager<IdentityUser> identityStoreManager,
            IDocumentManager<RolesForAccount> roleForAccountStoreManager,
            IDocumentManager<ClaimsForAccount> claimForAccountStoreManager,
            IIdentityNotifications<IdentityUser, IdentityRole> notificator)
            : this(identityStoreManager, roleForAccountStoreManager, claimForAccountStoreManager)
        {
            Notificator = notificator;
        }
        public Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken) 
            => CreateOrUpdateAsync(user, false, cancellationToken);
        private async Task<IdentityResult> CreateOrUpdateAsync(IdentityUser user, bool needsUpdate, CancellationToken cancellationToken)
        {
            if (user.Id == default)
                user.Id = Guid.NewGuid().ToString();
            Task<bool> a, b;
            await Task.WhenAll(a = IdentityStoreManager.UpdateAsync(user),
                b = IdentityStoreManager.UpdateAsync(user, Installation.Inst00)).NoContext();
            if (Notificator != default && a.Result && b.Result)
                _ = Notificator
                    .CreatedOrUpdatedAsync(needsUpdate,
                        user,
                        ServiceLocator.GetService<UserManager<IdentityUser>>(),
                        ServiceLocator.GetService<RoleManager<IdentityRole>>());
            return a.Result && b.Result ? IdentityResult.Success : IdentityResult.Failed();
        }
        public async Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            var result = await IdentityStoreManager.DeleteAsync(user).NoContext();
            return result ? IdentityResult.Success : IdentityResult.Failed();
        }
        public Task<IdentityUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
            => IdentityStoreManager.FirstOrDefaultAsync(default, x => x.Id == userId, Installation.Inst00);
        public Task<IdentityUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
            => IdentityStoreManager.FirstOrDefaultAsync(default, x => x.NormalizedUserName == normalizedUserName);
        public Task<string> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.NormalizedUserName);
        public Task<string> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            if (user.Id == default)
                user.Id = Guid.NewGuid().ToString();
            return Task.FromResult(user.Id);
        }
        public Task<string> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.UserName);
        public Task SetNormalizedUserNameAsync(IdentityUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }
        public Task SetUserNameAsync(IdentityUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }
        public Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken)
            => CreateOrUpdateAsync(user, true, cancellationToken);
        public void Dispose()
        {
        }
    }
}