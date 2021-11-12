using Microsoft.AspNetCore.Identity;
using Rystem.Identity;

namespace Rystem.Test.Accounting.Models
{
    public class IdentityServices : IUserTwoFactorTokenProvider<IdentityUser>
    {
        public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<IdentityUser> manager, IdentityUser user)
        {
            return Task.FromResult(true);
        }

        public Task<string> GenerateAsync(string purpose, UserManager<IdentityUser> manager, IdentityUser user)
        {
            return Task.FromResult("555");
        }

        public Task<bool> ValidateAsync(string purpose, string token, UserManager<IdentityUser> manager, IdentityUser user)
        {
            return Task.FromResult(true);
        }
    }
    public class IdentityNotificator : IIdentityNotifications<IdentityUser, IdentityRole>
    {
        public async Task CreatedOrUpdatedAsync(bool isUpdated, IdentityUser user, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin").NoContext())
            {
                await roleManager.CreateAsync(new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = DateTime.UtcNow.ToString() }).NoContext();
            }
            if (!await userManager.IsInRoleAsync(user, "Admin"))
            {
                await userManager.AddToRoleAsync(user, "Admin").NoContext();
            }
        }
    }
}
