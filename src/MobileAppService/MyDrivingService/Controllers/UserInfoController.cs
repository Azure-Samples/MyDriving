// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.Azure.Mobile.Server.Config;
using MyDrivingService.Helpers;
using MyDrivingService.Models;

namespace MyDrivingService.Controllers
{
    [MobileAppController]
    public class UserInfoController : ApiController
    {
        // GET api/UserInfo
        [Authorize]
        public async Task<MyDriving.DataObjects.UserProfile> Get()
        {
            //return the current authenticated user profile
            ClaimsPrincipal user = User as ClaimsPrincipal;
            bool? isAuthenticated = user?.Identity?.IsAuthenticated;

            if (!isAuthenticated.GetValueOrDefault())
            {
                return null;
            }

            var userId = string.Empty;
            // Get the credentials for the logged-in user.
            var fbCredentials = await user.GetAppServiceIdentityAsync<FacebookCredentials>(Request);
            var msCredentials = await user.GetAppServiceIdentityAsync<MicrosoftAccountCredentials>(Request);
            var twitterCredentials = await user.GetAppServiceIdentityAsync<TwitterCredentials>(Request);

            string first = string.Empty, last = string.Empty, profile = string.Empty;
            userId = await IdentitiyHelper.FindSidAsync(User, Request);
            if (fbCredentials?.UserClaims?.Count() > 0)
            {
                FillDataFromFacebook(fbCredentials, out first, out last, out profile);
            }
            else if (msCredentials?.UserClaims?.Count() > 0)
            {
                FillDataFromMS(msCredentials, out first, out last, out profile);
            }
            else if (twitterCredentials?.UserClaims?.Count() > 0)
            {
                FillDataFromTwitter(twitterCredentials, out first, out last, out profile);
            }
            else
            {
                return null;
            }


            var context = new MyDrivingContext();

            var curUser = context.UserProfiles.FirstOrDefault(u => u.UserId == userId);

            if (curUser == null)
            {
                curUser = new MyDriving.DataObjects.UserProfile
                {
                    UserId = userId,
                    ProfilePictureUri = profile,
                    FirstName = first,
                    LastName = last
                };
                context.UserProfiles.Add(curUser);
            }
            else
            {
                curUser.FirstName = first;
                curUser.LastName = last;
                curUser.ProfilePictureUri = profile;
            }

            await context.SaveChangesAsync();

            return curUser;
        }

        void FillDataFromFacebook(FacebookCredentials credentials, out string first, out string last, out string profile)
        {
            first = credentials.UserClaims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value ?? string.Empty;
            last = credentials.UserClaims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value ?? string.Empty;
            var id = credentials.UserClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ??
                     string.Empty;

            profile = $"https://graph.facebook.com/{id}/picture?type=large";
        }

        void FillDataFromTwitter(TwitterCredentials credentials, out string first, out string last, out string profile)
        {
            first = credentials.UserClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? string.Empty;
            profile =
                credentials.UserClaims.FirstOrDefault(c => c.Type == "urn:twitter:profile_image_url_https")?.Value ??
                string.Empty;

            //get largest image from twitter
            profile = profile.Replace("_normal", string.Empty);
            last = string.Empty;
        }


        void FillDataFromMS(MicrosoftAccountCredentials credentials, out string first, out string last,
            out string profile)
        {
            first = credentials.UserClaims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value ?? string.Empty;
            last = credentials.UserClaims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value ?? string.Empty;
            var id = credentials.UserClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ??
                     string.Empty;

            profile = $"https://apis.live.net/v5.0/{id}/picture";
        }
    }
}