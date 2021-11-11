using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.Identity
{
    internal partial class RystemIdentityStore : IUserLoginStore<IdentityUser>
    {
        internal const char Separator = '$';
        private static string GetId(string loginProvider, string providerKey)
            => $"{loginProvider}{Separator}{providerKey}";
        public Task AddLoginAsync(IdentityUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            user.Id = GetId(login.LoginProvider, login.ProviderKey);
            return CreateAsync(user, cancellationToken);
        }

        public Task<IdentityUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
            => FindByIdAsync(GetId(loginProvider, providerKey), cancellationToken);
        public Task<IList<UserLoginInfo>> GetLoginsAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            IList<UserLoginInfo> infos = new List<UserLoginInfo>();
            string[] id = user.ToString().Split(Separator);
            infos.Add(new UserLoginInfo(id[0], id[1], user.NormalizedUserName));
            return Task.FromResult(infos);
        }
        public Task RemoveLoginAsync(IdentityUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
            => _ = DeleteAsync(new IdentityUser() { Id = GetId(loginProvider, providerKey), NormalizedUserName = user.NormalizedUserName }, cancellationToken);
    }
}
