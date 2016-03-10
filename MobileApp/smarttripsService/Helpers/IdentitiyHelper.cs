using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace smarttripsService.Helpers
{
    public static class IdentitiyHelper
    {

        public static string FindSid(this IPrincipal claimsPrincipal)
        {
            var claim = claimsPrincipal as ClaimsPrincipal;
            if (claim == null)
                return string.Empty;

            var first  = claim.FindFirst(ClaimTypes.NameIdentifier);
            if (first == null)
                return string.Empty;

            return first.Value ?? string.Empty;
        }

    }
}