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
        const string FacebookGraphUrl = "https://graph.facebook.com/v2.5/me?fields=first_name%2Clast_name%2Cpicture%7Burl%7D&access_token=";
        const string MicrosoftUrl = "https://apis.live.net/v5.0/me?access_token=";
        const string TwitterUrl = "https://api.twitter.com/1.1/users/show.json?user_id=";

        // GET api/UserInfo
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

                    await FillDataFromFacebook(userProfile, fbCredentials.AccessToken);
                }
                else if (msCredentials?.UserClaims?.Count() > 0)
                {
                    userId = msCredentials.UserId;
                    await FillDataFromMS(userProfile, msCredentials.AccessToken);
                }
                else
                {
                    userId = twitterCredentials.UserId;
                    var settings = Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();

                    await FillDataFromTwitter(userProfile, twitterCredentials, settings["TwitterKey"], settings["TwitterSecret"]);
                }


                var context = new smarttripsContext();
                var curUser = context.Users.Where(u => u.UserId == userId).FirstOrDefault();

                if (curUser == null && userProfile != null)
                {
                    context.Users.Add(new MyTrips.DataObjects.UserProfile
                    {
                        UserId = userId,
                        ProfilePictureUri = userProfile.ProfilePictureUri,
                        FirstName = userProfile.FirstName,
                        LastName = userProfile.LastName
                    });

                    context.SaveChanges();
                }
            }
            return userProfile;
        }

        private static async Task FillDataFromFacebook(DataObjects.UserProfile userProfile, string token)
        {
            // Create a query string with the Facebook access token.
            var fbRequestUrl = FacebookGraphUrl + token;

            using (var client = new HttpClient())
            {
                // Request the current user info from Facebook.
                var resp = await client.GetAsync(fbRequestUrl);
                resp.EnsureSuccessStatusCode();

                string fbInfo = await resp.Content.ReadAsStringAsync();

                JObject fbObject = JObject.Parse(fbInfo);
                userProfile.FirstName = fbObject.GetValue("first_name")?.ToString() ?? string.Empty;
                userProfile.LastName = fbObject.GetValue("last_name")?.ToString() ?? string.Empty;
                var picture = fbObject.GetValue("picture")?.ToString() ?? string.Empty;
                var data = JObject.Parse(picture)?.GetValue("data")?.ToString() ?? string.Empty;
                var url = JObject.Parse(data)?.GetValue("url")?.ToString() ?? string.Empty;
                userProfile.ProfilePictureUri = url;
            }
        }

        static async Task FillDataFromTwitter(DataObjects.UserProfile userProfile, TwitterCredentials credentials, string key, string secret)
        {
            var twitterId = ulong.Parse(credentials.UserId.Substring(credentials.UserId.IndexOf(':') + 1));
            
            var auth = new MvcAuthorizer
            {
                CredentialStore = new LinqToTwitter.SessionStateCredentialStore
                {
                    OAuthToken = credentials.AccessToken,
                    OAuthTokenSecret = credentials.AccessTokenSecret,
                    ConsumerKey = key,
                    ConsumerSecret = secret,
                    UserID = twitterId
                }
            };

            await auth.AuthorizeAsync();

            var ctx = new TwitterContext(auth);

            var userResponse =
              await
              (from user in ctx.User
               where user.Type == UserType.Lookup &&
                     user.UserID == twitterId
               select user)
              .ToListAsync();

            if (userResponse.Count == 0)
                return;

            var twitterUser = userResponse[0];
            userProfile.FirstName = twitterUser.Name;
            userProfile.LastName = string.Empty;
            userProfile.ProfilePictureUri = twitterUser.ProfileImageUrlHttps;

        }


        private static async Task FillDataFromMS(DataObjects.UserProfile userProfile, string token)
        {
            // Create a query string with the Facebook access token.
            var msRequestUrl = MicrosoftUrl + token;
            var pictureUrl = string.Empty;
            using (var client = new System.Net.Http.HttpClient())
            {
                var resp = await client.GetAsync(msRequestUrl);
                resp.EnsureSuccessStatusCode();
                string info = await resp.Content.ReadAsStringAsync();
                JObject fbObject = JObject.Parse(info);
                userProfile.FirstName = fbObject.GetValue("first_name")?.ToString() ?? string.Empty;
                userProfile.LastName = fbObject.GetValue("last_name")?.ToString() ?? string.Empty;
                string id = fbObject.GetValue("id")?.ToString() ?? string.Empty;
              
                if(!string.IsNullOrWhiteSpace(id))
                    pictureUrl = string.Format("https://apis.live.net/v5.0/{0}/picture", id);

                userProfile.ProfilePictureUri = pictureUrl;
            }

            if (string.IsNullOrWhiteSpace(pictureUrl))
                return;

            //request for the profile picture
            using (var client = new HttpClient())
            {
                var resp = await client.GetAsync(pictureUrl);
                resp.EnsureSuccessStatusCode();
                var picture = await resp.Content.ReadAsByteArrayAsync();
                userProfile.ProfilePicture = picture;
            }
        }

    }


}
