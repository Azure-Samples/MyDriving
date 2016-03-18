// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;

namespace MyDriving.Interfaces
{
    public interface IAuthentication
    {
        Task<MobileServiceUser> LoginAsync(IMobileServiceClient client, MobileServiceAuthenticationProvider provider);
        void ClearCookies();
    }
}