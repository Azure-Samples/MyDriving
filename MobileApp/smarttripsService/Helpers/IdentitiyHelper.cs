using Microsoft.Azure.Mobile.Server.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;

namespace smarttripsService.Helpers
{
    public static class IdentitiyHelper
    {

        public static async Task<string> FindSidAsync(IPrincipal claimsPrincipal, HttpRequestMessage request)
        {
            var principal = claimsPrincipal as ClaimsPrincipal;
            if (principal == null)
                return string.Empty;

            var provider = principal.FindFirst("http://schemas.microsoft.com/identity/claims/identityprovider").Value;

            ProviderCredentials creds = null;
            if (string.Equals(provider, "facebook", StringComparison.OrdinalIgnoreCase))
            {
                creds = await claimsPrincipal.GetAppServiceIdentityAsync<FacebookCredentials>(request);
            }
            else if (string.Equals(provider, "microsoftaccount", StringComparison.OrdinalIgnoreCase))
            {
                creds = await claimsPrincipal.GetAppServiceIdentityAsync<MicrosoftAccountCredentials>(request);
            }
            else if (string.Equals(provider, "twitter", StringComparison.OrdinalIgnoreCase))
            {
                creds = await claimsPrincipal.GetAppServiceIdentityAsync<TwitterCredentials>(request);
            }

            if (creds == null)
                return string.Empty;
            

            var finalId =  $"{creds.Provider}:{creds.UserClaims.First(c => c.Type == ClaimTypes.NameIdentifier).Value}";
            return finalId;
        }

    }
}