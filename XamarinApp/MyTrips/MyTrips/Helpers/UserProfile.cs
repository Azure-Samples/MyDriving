using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using MyTrips.Utils;
using MyTrips.DataObjects;

namespace MyTrips.Helpers
{
    public class UserProfileHelper
    {
        //returns info for the authenticated user
        public static async Task<UserProfile> GetUserProfileAsync(IMobileServiceClient client)
        {
            var userprof =
            await client.InvokeApiAsync<UserProfile>(
                "UserInfo",
                System.Net.Http.HttpMethod.Get,
                null);

            Settings.Current.UserFirstName = userprof?.FirstName ?? string.Empty;
            Settings.Current.UserLastName = userprof?.LastName ?? string.Empty;
            Settings.Current.UserProfileUrl = userprof?.ProfilePictureUri ?? string.Empty;

            if (string.IsNullOrWhiteSpace(userprof?.ProfilePictureUri))
            {
                //should set a default here
            }

            return userprof;
        }
    }
}
