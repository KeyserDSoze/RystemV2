using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.Identity
{
    internal partial class RystemIdentityStore : IUserRoleStore<IdentityUser>
    {
        private List<string> Roles;
        public async Task AddToRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken)
        {
            (await GetRolesAsync(user, cancellationToken).NoContext())
                .Add(roleName);
            await RoleForAccountStoreManager
                    .UpdateAsync(new RolesForAccount { Id = user.Id, Role = roleName }).NoContext();
        }

        public async Task<IList<string>> GetRolesAsync(IdentityUser user, CancellationToken cancellationToken)
            => Roles ??= (await RoleForAccountStoreManager.GetAsync(default, x => x.Id == user.Id).NoContext())
                .Select(x => x.Role)
                .ToList();

        public async Task<IList<IdentityUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            List<IdentityUser> users = new();
            foreach (var role in await RoleForAccountStoreManager.GetAsync(default, x => x.Role == roleName).NoContext())
                if (!cancellationToken.IsCancellationRequested)
                    users.Add(await FindByIdAsync(role.Id, cancellationToken).NoContext());
            return users;
        }

        public async Task<bool> IsInRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken)
            => (await GetRolesAsync(user, cancellationToken).NoContext()).Contains(roleName);

        public Task RemoveFromRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken)
        {
            Roles?.Remove(roleName);
            return RoleForAccountStoreManager.DeleteAsync(new RolesForAccount { Id = user.Id, Role = roleName });
        }
    }
}