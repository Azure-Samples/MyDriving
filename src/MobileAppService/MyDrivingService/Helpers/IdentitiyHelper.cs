// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.ApplicationInsights;

namespace MyDrivingService.Helpers
{
    public static class IdentitiyHelper
    {
        public static async Task<string> FindSidAsync(IPrincipal claimsPrincipal, HttpRequestMessage request)
        {
            var aiTelemetry = new TelemetryClient();
            var principal = claimsPrincipal as ClaimsPrincipal;
            if (principal == null)
            {
                aiTelemetry.TrackEvent("FindSidAsync: ClaimsPrincipal is null!");
                return string.Empty;
            }

            var match = principal.FindFirst("http://schemas.microsoft.com/identity/claims/identityprovider");
            string provider;
            if (match != null)
                provider = match.Value;
            else
            {
                aiTelemetry.TrackEvent("FindSidAsync: Can't find identity provider");
                return string.Empty;
            }

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
            {
                aiTelemetry.TrackEvent("FindSidAsync: Credentials not found");
                return string.Empty;
            }


            var finalId = $"{creds.Provider}:{creds.UserClaims.First(c => c.Type == ClaimTypes.NameIdentifier).Value}";
            return finalId;
        }
    }
}