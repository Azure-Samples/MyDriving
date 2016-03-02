using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;

namespace MyTrips.Helpers
{
    public class UserProfileHelper
    {
        //returns info for the authenticated user
        public static async Task<UserProfile> GetUserProfileAsync(MobileServiceClient client)
        {
            UserProfile userprof =
            await client.InvokeApiAsync<UserProfile>(
                "UserInfo",
                System.Net.Http.HttpMethod.Get,
                null);

            return userprof;
        }
    }
}

namespace MyTrips
{
    public class UserProfile
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePictureUri { get; set; }
        public byte[] ProfilePicture { get; set; }
    }
}
