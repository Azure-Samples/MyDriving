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
            if (Settings.Current.TokenExpiration <= DateTime.UtcNow)
            {
                var client = ServiceLocator.Instance.Resolve<IAzureClient>()?.Client;
                try
                {
                    JArray response = (JArray)await client.InvokeApiAsync("/.auth/me", HttpMethod.Get, null);

                    JObject exp = (JObject)(response.First);
                    var expiration = exp["expires_on"].Value<string>();

                    Settings.Current.TokenExpiration = DateTime.Parse(expiration, null, System.Globalization.DateTimeStyles.AssumeUniversal);
                }
                catch (System.Exception e)
                {
                    Logger.Instance.WriteLine("CheckIsAuthTokenValid: " + e.Message);
                }
            }
        }
    }
}