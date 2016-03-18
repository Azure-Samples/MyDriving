// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.WindowsAzure.MobileServices;

namespace MyDriving.AzureClient
{
    public class AzureClient : IAzureClient
    {
        const string DefaultMobileServiceUrl = "https://mydriving.azurewebsites.net";
        IMobileServiceClient _client;
        string _mobileServiceUrl;
        public IMobileServiceClient Client => _client ?? (_client = CreateClient());


        IMobileServiceClient CreateClient()
        {
            _client = new MobileServiceClient(DefaultMobileServiceUrl, new AuthHandler())
            {
                SerializerSettings = new MobileServiceJsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                    CamelCasePropertyNames = true
                }
            };
            return _client;
        }
    }
}