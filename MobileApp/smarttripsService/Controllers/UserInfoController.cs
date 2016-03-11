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
        [HttpGet]
        public async Task<DataObjects.UserProfile> Get()
        {
            DataObjects.UserProfile userProfile = new DataObjects.UserProfile();
            //return the current authenticated user profile
            ClaimsPrincipal user = User as ClaimsPrincipal;
            bool? isAuthenticated = user?.Identity?.IsAuthenticated;
            if (isAuthenticated == true)
            {
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
                else
                {
                    userId = twitterCredentials.UserId;
                    var settings = Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();

                    await FillDataFromTwitter(userProfile, twitterCredentials, settings["TwitterKey"], settings["TwitterSecret"]);
                }

                var context = new smarttripsContext();

                try
                {
                    var curUser = context.UserProfiles.Where(u => u.UserId == userId).FirstOrDefault();
               
                    if (curUser == null && userProfile != null)
                    {
                        context.UserProfiles.Add(new MyTrips.DataObjects.UserProfile
                        {
                            UserId = userId,
                            ProfilePictureUri = userProfile.ProfilePictureUri,
                            FirstName = userProfile.FirstName,
                            LastName = userProfile.LastName
                        });

                        context.SaveChanges();
                    }
                }
                catch(Exception ex)
                {

                }
            }
            return userProfile;
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

        static async Task FillDataFromTwitter(DataObjects.UserProfile userProfile, TwitterCredentials credentials, string key, string secret)
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

            //request for the profile picture
            using (var client = new HttpClient())
            {
                var resp = await client.GetAsync(userProfile.ProfilePictureUri);
                resp.EnsureSuccessStatusCode();
                var picture = await resp.Content.ReadAsByteArrayAsync();
                userProfile.ProfilePicture = picture;
            }
        }
    }

}
