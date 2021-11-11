using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using System;

namespace Rystem.Identity
{
    public class RystemIdentityOptions
    {
        public string AccountTableName { get; set; }
        public string RoleTableName { get; set; } 
        public string ClaimsTableName { get; set; }
        public string AccountRolesTableName { get; set; }
        public bool ExternalIsAutoregistered { get; set; }
        public Action<MicrosoftAccountOptions> MicrosoftConfigureOptions { get; set; }
        public bool HasMicrosoftAsExternalLogin => MicrosoftConfigureOptions != null;
        public Action<GoogleOptions> GoogleConfigureOptions { get; set; }
        public bool HasGoogleAsExternalLogin => GoogleConfigureOptions != null;
        public Action<FacebookOptions> FacebookConfigureOptions { get; set; }
        public bool HasFacebookAsExternalLogin => FacebookConfigureOptions != null;
    }
}