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

namespace smarttripsService.Controllers
{
    [MobileAppController]
    public class UserInfoController : ApiController
    {
        const string FacebookGraphUrl = "https://graph.facebook.com/v2.5/me?fields=first_name%2Clast_name%2Cpicture%7Burl%7D&access_token=";

        // GET api/ManageUser
        public async Task<UserProfile> Get()
        {
            UserProfile userProfile = new UserProfile();
            //return the current authenticated user profile
            ClaimsPrincipal user = User as ClaimsPrincipal;
            bool? isAuthenticated = user?.Identity?.IsAuthenticated;
            if (isAuthenticated == true)
            {
                // Get the credentials for the logged-in user.
                var fbCredentials = await user.GetAppServiceIdentityAsync<FacebookCredentials>(Request);
                //var twitterCredentials = await user.GetAppServiceIdentityAsync<TwitterCredentials>(request);

                if (fbCredentials?.UserClaims?.Count() > 0)
                {
                    await FillDataFromFacebook(userProfile, fbCredentials.AccessToken);
                }
                // else if (twitterCredentials?.Claims?.Count > 0)
                {
                    //    
                }
            }
            return userProfile;
        }

        private static async Task FillDataFromFacebook(UserProfile userProfile, string token)
        {
            // Create a query string with the Facebook access token.
            var fbRequestUrl = FacebookGraphUrl + token;

            using (var client = new System.Net.Http.HttpClient())
            {
                // Request the current user info from Facebook.
                var resp = await client.GetAsync(fbRequestUrl);
                resp.EnsureSuccessStatusCode();

                string fbInfo = await resp.Content.ReadAsStringAsync();

                JObject fbObject = JObject.Parse(fbInfo);
                userProfile.FirstName = fbObject.GetValue("first_name")?.ToString();
                userProfile.LastName = fbObject.GetValue("last_name")?.ToString();
                var picture = fbObject.GetValue("picture")?.ToString();
                var data = JObject.Parse(picture)?.GetValue("data")?.ToString();
                var url = JObject.Parse(data)?.GetValue("url")?.ToString();
                userProfile.ProfilePictureUri = url;
            }
        }
    }


}
