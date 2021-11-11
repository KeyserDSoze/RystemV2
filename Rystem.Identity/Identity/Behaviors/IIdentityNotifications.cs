using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Rystem.Identity
{
    public interface IIdentityNotifications<TUser, TRole> 
        where TUser : IdentityUser
        where TRole : IdentityRole
    {
        Task CreatedOrUpdatedAsync(bool isUpdated, TUser user, UserManager<TUser> userManager, RoleManager<TRole> roleManager);
    }
}