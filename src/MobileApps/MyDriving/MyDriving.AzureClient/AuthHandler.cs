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
using MyDriving.Utils.Interfaces;
using MyDriving.Utils.Helpers;

namespace MyDriving.AzureClient
{
    public static class AuthenticationManager
    {
        static readonly object authLock = new object();
        static bool isAuthenticating;

        public static bool IsAuthenticating
        {
            get
            {
                lock (authLock)
                {
                    return isAuthenticating;
                }
            }
            set
            {
                lock (authLock)
                {
                    isAuthenticating = value;
                }
            }
        }
    }

    class AuthHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var authentication = ServiceLocator.Instance.Resolve<IAuthentication>();
            var client = ServiceLocator.Instance.Resolve<IAzureClient>()?.Client as MobileServiceClient;

            if (client == null)
            {
                throw new InvalidOperationException( "Make sure to set the ServiceLocator has an instance of IAzureClient");
            }

            // Cloning the request, in case we need to send it again
            var clonedRequest = await CloneRequest(request);
            var response = await base.SendAsync(clonedRequest, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                //If there is a progress dialog open, hide it so that the UI isn't blocked 
                ProgressDialogManager.HideProgressDialog();

                // The user is not logged in - we got a 401 (token expired)
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

                    if (AuthenticationManager.IsAuthenticating)
                    {
                        return response;
                    }
                    else
                    {
                        AuthenticationManager.IsAuthenticating = true;
                    }

                    // Prompt the user to log-in again 
                    var user = await authentication.LoginAsync(client, accountType);

                    if (user != null) 
                    {
                        //TODO: There is still a problem with this code because it never gets executed after the user gets re-logged in
                        //from the main UI thread...need to figure out how to address this.
                        Settings.Current.AuthToken = user?.MobileServiceAuthenticationToken ?? string.Empty;
                        Settings.Current.AzureMobileUserId = user?.UserId ?? string.Empty;

                        AuthenticationManager.IsAuthenticating = false;

                        //Reshow the progress dialog if needed
                        ProgressDialogManager.ShowProgressDialog();

                        // Clone the request
                        clonedRequest = await CloneRequest(request);

                        // Set the authentication header
                        clonedRequest.Headers.Remove("X-ZUMO-AUTH");
                        clonedRequest.Headers.Add("X-ZUMO-AUTH", user.MobileServiceAuthenticationToken);

                        // Resend the request
                        response = await base.SendAsync(clonedRequest, cancellationToken);
                    }
                }
                catch 
                {
                    AuthenticationManager.IsAuthenticating = false;

                    // Exception occurred, so return original response
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
 