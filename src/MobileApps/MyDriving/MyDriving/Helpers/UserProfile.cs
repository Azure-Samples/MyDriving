// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using MyDriving.Utils;
using MyDriving.DataObjects;

namespace MyDriving.Helpers
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
            Settings.Current.UserUID = userprof?.UserId ?? string.Empty;

            if (string.IsNullOrWhiteSpace(userprof?.ProfilePictureUri))
            {
                Settings.Current.UserProfileUrl = "http://appstudio.windows.com/Content/img/temp/icon-user.png";
            }

            return userprof;
        }
    }
}