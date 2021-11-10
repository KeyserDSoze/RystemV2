using Microsoft.AspNetCore.Identity;
using Rystem.Business.Document;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.Business.Identity
{
    internal class RystemRoleStore : IRoleStore<IdentityRole>
    {
        private readonly IDocumentManager<IdentityRole> RoleStoreManager;
        public RystemRoleStore(IDocumentManager<IdentityRole> roleStoreManager)
        {
            RoleStoreManager = roleStoreManager;
        }
        public async Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            var result = await RoleStoreManager.UpdateAsync(role).NoContext();
            return result ? IdentityResult.Success : IdentityResult.Failed();
        }
        public async Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            var result = await RoleStoreManager.DeleteAsync(role).NoContext();
            return result ? IdentityResult.Success : IdentityResult.Failed();
        }
        public void Dispose()
        {
        }
        public Task<IdentityRole> FindByIdAsync(string roleId, CancellationToken cancellationToken) 
            => RoleStoreManager.FirstOrDefaultAsync(default, x => x.Id == roleId);
        public Task<IdentityRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
            => RoleStoreManager.FirstOrDefaultAsync(default, x => x.NormalizedName == normalizedRoleName);
        public Task<string> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken) 
            => Task.FromResult(role.NormalizedName);
        public Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken cancellationToken)
            => Task.FromResult(role.Id);
        public Task<string> GetRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
            => Task.FromResult(role.Name);
        public Task SetNormalizedRoleNameAsync(IdentityRole role, string normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }
        public Task SetRoleNameAsync(IdentityRole role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.CompletedTask;
        }
        public Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken cancellationToken) 
            => CreateAsync(role, cancellationToken);
    }
}
