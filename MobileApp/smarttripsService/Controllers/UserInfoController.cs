using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.Azure.Mobile.Server.Config;
using Newtonsoft.Json.Linq;
using smarttripsService.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Http;
using MyTrips.DataObjects;
using smarttripsService.Models;
using LinqToTwitter;
using System.Configuration;

namespace smarttripsService.Controllers
{
    [MobileAppController]
    public class UserInfoController : ApiController
    {
        // GET api/UserInfo
        //[Authorize]
        public async Task<MyTrips.DataObjects.UserProfile> Get()
        {
            DataObjects.UserProfile userProfile = new DataObjects.UserProfile();
            //return the current authenticated user profile
            ClaimsPrincipal user = User as ClaimsPrincipal;
            bool? isAuthenticated = user?.Identity?.IsAuthenticated;
            
            if(!isAuthenticated)
            {
                return null;
            }
            
            var userId = string.Empty;
            // Get the credentials for the logged-in user.
            var fbCredentials = await user.GetAppServiceIdentityAsync<FacebookCredentials>(Request);
            var msCredentials = await user.GetAppServiceIdentityAsync<MicrosoftAccountCredentials>(Request);
            var twitterCredentials = await user.GetAppServiceIdentityAsync<TwitterCredentials>(Request);

            if (fbCredentials?.UserClaims?.Count() > 0)
            {
                userId = fbCredentials.UserId;

                await FillDataFromFacebook(userProfile, fbCredentials);
            }
            else if (msCredentials?.UserClaims?.Count() > 0)
            {
                userId = msCredentials.UserId;
                await FillDataFromMS(userProfile, msCredentials);
            }
            else if (twitterCredentials?.UserClaims?.Count() > 0)
            {
                userId = twitterCredentials.UserId;
                var settings = Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();

                await FillDataFromTwitter(userProfile, twitterCredentials);
            }

            if(userProfile == null)
                return null;

            var context = new smarttripsContext();
            
            var curUser = context.UserProfiles.Where(u => u.UserId == userId).FirstOrDefault();
        
            if (curUser == null)
            {
                context.UserProfiles.Add(new MyTrips.DataObjects.UserProfile
                {
                    UserId = userId,
                    ProfilePictureUri = userProfile.ProfilePictureUri,
                    FirstName = userProfile.FirstName,
                    LastName = userProfile.LastName
                });
            }
            else
            {
                curUser.FirstName = userProfile.FirstName;
                curUser.LastName = userProfile.LastName;
                curUser.ProfilePictureUri.ProfilePictureUri;
            }
                
            context.SaveChanges();
            
            return curUser;
        }

        private static async Task FillDataFromFacebook(DataObjects.UserProfile userProfile, FacebookCredentials credentials)
        {

            var first = credentials.UserClaims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value ?? string.Empty;
            var last = credentials.UserClaims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value ?? string.Empty;
            var id = credentials.UserClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            var profile = $"https://graph.facebook.com/{id}/picture?type=large";

            userProfile.FirstName = first;
            userProfile.LastName = last;
            userProfile.ProfilePictureUri = profile;
        }

        static async Task FillDataFromTwitter(DataObjects.UserProfile userProfile, TwitterCredentials credentials)
        {
            var name = credentials.UserClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? string.Empty;
            var profile = credentials.UserClaims.FirstOrDefault(c => c.Type == "urn:twitter:profile_image_url_https")?.Value ?? string.Empty;

            //get largest image from twitter
            profile = profile.Replace("_normal", string.Empty);
            userProfile.FirstName = name;
            userProfile.LastName = string.Empty;
            userProfile.ProfilePictureUri = profile;

        }


        private static async Task FillDataFromMS(DataObjects.UserProfile userProfile, MicrosoftAccountCredentials credentials)
        {
           
            var first = credentials.UserClaims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value ?? string.Empty;
            var last = credentials.UserClaims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value ?? string.Empty;
            var id = credentials.UserClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            var profile = $"https://apis.live.net/v5.0/{id}/picture";

            userProfile.FirstName = first;
            userProfile.LastName = last;
            userProfile.ProfilePictureUri = profile;
        }

    }


}
