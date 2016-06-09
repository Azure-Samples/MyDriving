using System;
using MyDriving.DataStore.Abstractions;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using MyDriving.Utils;
using System.Collections.Generic;
using MyDriving.DataObjects;
using Plugin.Connectivity;
using MyDriving.AzureClient;
using System.Threading;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using MyDriving.Utils.Interfaces;
using MyDriving.Utils.Helpers;

namespace MyDriving.DataStore.Azure
{
    public class AuthenticationStore
    {
        static IAuthentication authentication;

        enum SyncResult
        {
            AuthError,
            OtherError,
            Success
        }

        static AuthenticationStore()
        {
            authentication = ServiceLocator.Instance.Resolve<IAuthentication>();
        }

        public static async Task<bool> AuthenticatedSyncAsync<T>(IMobileServiceSyncTable<T> table, string dataObjIdentifier) where T : IBaseDataObject
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                Logger.Instance.Track("Unable to sync items, we are offline");
                return false;
            }

            var provider = GetProviderType();

            //Send changes to the mobile service
            var syncResult = await SyncAsync(table, dataObjIdentifier);
            var result = syncResult == SyncResult.Success;

            //If the token is expired or is invalid, then we need to either refresh the token or prompt the user to log back in
            if (syncResult == SyncResult.AuthError)
            {
                //For MSA, attempt to refresh the token if we haven't already, so that user doesn't have to log back in again
                //This isn't needed for Facebook since the token doesn't expire for 60 days; similarly, for Twitter, the token never expires
                if (provider == MobileServiceAuthenticationProvider.MicrosoftAccount)
                {
                    if (RefreshToken(table.MobileServiceClient))
                    {
                        //Resend the changes to the mobile service
                        syncResult = await SyncAsync(table, dataObjIdentifier);
                        return syncResult == SyncResult.Success;
                    }
                }

                //Otherwise if refreshing the token failed or Facebook\Twitter is being used, prompt the user to log back in via the login screen
                //First, make sure there isn't a progress dialog being displayed since this will block the main ui thread
                ProgressDialogManager.HideProgressDialog();

                var user = await authentication.LoginAsync(table.MobileServiceClient, provider);

                //Redisplay the progress dialog if needed
                ProgressDialogManager.ShowProgressDialog();

                if (user != null)
                {
                    //Resend the changes to the mobile service
                    syncResult = await SyncAsync(table, dataObjIdentifier);
                    return syncResult == SyncResult.Success;
                }
            }

            return result;
        }

        private static async Task<SyncResult> SyncAsync<T>(IMobileServiceSyncTable<T> table, string dataObjIdentifier) where T: IBaseDataObject
        {
            SyncResult result = SyncResult.Success;

            try
            {
                //Note: A pull will implicitly invoke a push if there are unpending local changes.  As a result, explicitly calling push before or after 
                //the pull is redundant - and in the scenario that the user is unauthenticated, this would needlessly prompt the user twice to login.
                //Also, when the implicit push occurs, this will push changes across the entire sync context (e.g. all tables).
                await table.PullAsync($"all{dataObjIdentifier}", table.CreateQuery());
            }
            catch (Exception e)
            {
                Logger.Instance.Track("SyncAsync: Unable to push/pull items: " + e.Message);

                if (e is MobileServicePushFailedException &&
                    ((MobileServicePushFailedException)e).PushResult.Status == MobileServicePushStatus.CancelledByAuthenticationError)
                {
                    result = SyncResult.AuthError;
                }
                else
                {
                    result = SyncResult.OtherError;
                }
            }

            return result;
        }

        private static bool RefreshToken(IMobileServiceClient client)
        {
            bool refreshSucceeded = false;

            try
            {
                JObject refreshJson = (JObject)client.InvokeApiAsync("/.auth/refresh", HttpMethod.Get, null).Result;
                string newToken = refreshJson["authenticationToken"].Value<string>();
                client.CurrentUser.MobileServiceAuthenticationToken = newToken;
                Settings.Current.AuthToken = newToken;
                refreshSucceeded = true;
            }
            catch (Exception e)
            {
                Logger.Instance.Report(e);
            }

            return refreshSucceeded;
        }

        private static MobileServiceAuthenticationProvider GetProviderType()
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

            return accountType;
        }
    }
}
