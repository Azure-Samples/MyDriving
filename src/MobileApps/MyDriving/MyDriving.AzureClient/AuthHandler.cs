// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading;
using MyDriving.Utils;
using System.Text;

namespace MyDriving.AzureClient
{
    class AuthHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var client = ServiceLocator.Instance.Resolve<IAzureClient>()?.Client as MobileServiceClient;
            if (client == null)
            {
                throw new InvalidOperationException(
                    "Make sure to set the ServiceLocator has an instance of IAzureClient");
            }

            // Cloning the request, in case we need to send it again
            var clonedRequest = await CloneRequest(request);
            var response = await base.SendAsync(clonedRequest, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Oh noes, user is not logged in - we got a 401
                // Log them in, this time hardcoded with Microsoft but you would
                // trigger the login presentation in your application
                try
                {
                    var accountType = MobileServiceAuthenticationProvider.MicrosoftAccount;
                    switch (Settings.Current.LoginAccount)
                    {
                        case LoginAccount.Facebook:
                            accountType = MobileServiceAuthenticationProvider.Facebook;
                            break;
                        case LoginAccount.Twitter:
                            accountType = MobileServiceAuthenticationProvider.Twitter;
                            break;
                    }
                    var user = await client.LoginAsync(accountType, null);
                    // we're now logged in again.

                    // Clone the request
                    clonedRequest = await CloneRequest(request);


                    Settings.Current.AzureMobileUserId = user.UserId;
                    Settings.Current.AuthToken = user.MobileServiceAuthenticationToken;

                    clonedRequest.Headers.Remove("X-ZUMO-AUTH");
                    // Set the authentication header
                    clonedRequest.Headers.Add("X-ZUMO-AUTH", user.MobileServiceAuthenticationToken);

                    // Resend the request
                    response = await base.SendAsync(clonedRequest, cancellationToken);
                }
                catch (InvalidOperationException)
                {
                    // user cancelled auth, so lets return the original response
                    return response;
                }
            }

            return response;
        }

        private async Task<HttpRequestMessage> CloneRequest(HttpRequestMessage request)
        {
            var result = new HttpRequestMessage(request.Method, request.RequestUri);
            foreach (var header in request.Headers)
            {
                result.Headers.Add(header.Key, header.Value);
            }

            if (request.Content != null && request.Content.Headers.ContentType != null)
            {
                var requestBody = await request.Content.ReadAsStringAsync();
                var mediaType = request.Content.Headers.ContentType.MediaType;
                result.Content = new StringContent(requestBody, Encoding.UTF8, mediaType);
                foreach (var header in request.Content.Headers)
                {
                    if (!header.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                    {
                        result.Content.Headers.Add(header.Key, header.Value);
                    }
                }
            }

            return result;
        }
    }
}