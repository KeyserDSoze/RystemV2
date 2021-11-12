using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Rystem.Identity.External
{
    internal static class BasicAutoRegistrationManager
    {
        public static async Task RegisterAsync<T>(ClaimsIdentity identity, string providerName)
            where T : IdentityUser, new()
        {
            if (identity.IsAuthenticated)
            {
                var userManager = ServiceLocator.GetService<UserManager<T>>();
                string id = $"{providerName}{RystemIdentityStore.Separator}{identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value}";
                if (await userManager.FindByIdAsync(id).NoContext() == default)
                {
                    string email = identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
                    var user = new T()
                    {
                        Id = id,
                        NormalizedUserName = email.ToUpper(),
                        Email = email,
                        UserName = email,
                        NormalizedEmail = email.ToUpper(),
                        AccessFailedCount = 0,
                        EmailConfirmed = true,
                        LockoutEnabled = false,
                        PhoneNumberConfirmed = true,
                        TwoFactorEnabled = false,
                        ConcurrencyStamp = DateTime.UtcNow.Ticks.ToString(),
                    };
                    await userManager.CreateAsync(user).NoContext();
                }
            }
        }
    }
}
