using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.Business.Identity
{
    internal partial class RystemIdentityStore : IUserLoginStore<IdentityUser>
    {
        private const char Separator = '$';
        public Task AddLoginAsync(IdentityUser user, UserLoginInfo login, CancellationToken cancellationToken)
            => UpdateAsync(new IdentityUser() { Id = $"{login.LoginProvider}{Separator}{login.ProviderKey}", NormalizedUserName = login.ProviderDisplayName }, cancellationToken);

        public Task<IdentityUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
            => FindByNameAsync($"{loginProvider}{Separator}{providerKey}", cancellationToken);

        public Task<IList<UserLoginInfo>> GetLoginsAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            IList<UserLoginInfo> infos = new List<UserLoginInfo>();
            string[] id = user.ToString().Split(Separator);
            infos.Add(new UserLoginInfo(id[0], id[1], user.NormalizedUserName));
            return Task.FromResult(infos);
        }

        public Task RemoveLoginAsync(IdentityUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
            => IdentityStoreManager.DeleteAsync(new IdentityUser() { Id = $"{loginProvider}-{providerKey}", NormalizedUserName = user.NormalizedUserName });
    }
}
