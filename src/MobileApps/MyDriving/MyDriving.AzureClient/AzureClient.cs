// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.WindowsAzure.MobileServices;
using System.Net.Http;
using System.Threading.Tasks;
using MyDriving.Utils;
using Newtonsoft.Json.Linq;
using System;

namespace MyDriving.AzureClient
{
    public class AzureClient : IAzureClient
    {
        const string DefaultMobileServiceUrl = "https://mydriving.azurewebsites.net";
        static IMobileServiceClient client;

        public IMobileServiceClient Client => client ?? (client = CreateClient());

        IMobileServiceClient CreateClient()
        {
            client = new MobileServiceClient(DefaultMobileServiceUrl, new AuthHandler())
            {
                SerializerSettings = new MobileServiceJsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                    CamelCasePropertyNames = true
                }
            };
            return client;
        }

        public static async Task CheckIsAuthTokenValid()
        {
            //Check if the access token is valid by sending a general request to mobile service
            var client = ServiceLocator.Instance.Resolve<IAzureClient>()?.Client;
            try
            {
                await client.InvokeApiAsync("/.auth/me", HttpMethod.Get, null);
            }
            catch { } //Eat any exceptions
        }
    }
}