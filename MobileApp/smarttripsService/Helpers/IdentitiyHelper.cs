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

        public static string FindSid(this IPrincipal claimsPrincipal) =>
            ((claimsPrincipal as ClaimsPrincipal)?.FindFirst(ClaimTypes.NameIdentifier)?.Value) ?? string.Empty;

    }
}