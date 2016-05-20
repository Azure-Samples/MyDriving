// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;

namespace MyDriving.Utils
{
    public abstract class AuthenticationProvider
    {
        readonly object changeAuthLock = new object();
        bool isAuthenticated = false;

        protected abstract Task<MobileServiceUser> ServerLoginAsync(IMobileServiceClient client, MobileServiceAuthenticationProvider provider);

        public abstract void ClearCookies();

        public async Task<MobileServiceUser> LoginAsync(IMobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            var user = await this.ServerLoginAsync(client, provider);
            IsAuthenticated= (user == null || user.MobileServiceAuthenticationToken == string.Empty);

            return user;
        }

        public bool IsAuthenticated
        {
            get { return isAuthenticated; }
            set
            {
                if (isAuthenticated != value)
                {
                    lock (changeAuthLock)
                    {
                        isAuthenticated = value;
                    }
                }
            }
        }
    }
}