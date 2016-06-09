// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using MyDriving.DataStore.Abstractions;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using MyDriving.Utils;
using MyDriving.DataObjects;
using System.Collections.Generic;
using System.Linq;
using MyDriving.AzureClient;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;

namespace MyDriving.DataStore.Azure
{
    public class AzureMobileClientHandler : IMobileServiceSyncHandler
    {
        public async Task<JObject> ExecuteTableOperationAsync(IMobileServiceTableOperation operation)
        {
            //JObject result = null;
            //try
            //{
                var result = await operation.ExecuteAsync();
            //}
            //catch (MobileServiceInvalidOperationException e)
            //{
            //    if (e.Response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            //    {
            //        //If the user isn't authenticated, clear all pending operations from the sync context's queue so that we don't
            //        //needlessly keep trying to send data requests to the backend.  Note that any unsynced data will still exist in the 
            //        //local Sqlite store, so that next time we attempt to sync with the backend, we'll try again to resend this data.
            //        operation.AbortPush();
            //    }
            //}

            return result;
        }

        public Task OnPushCompleteAsync(MobileServicePushCompletionResult result)
        {
            return Task.FromResult(0);
        }
    }

    public class StoreManager : IStoreManager
    {
        #region IStoreManager implementation

        MobileServiceSQLiteStore store;

        public async Task InitializeAsync()
        {
            if (IsInitialized)
                return;

            //Get our current client, only ever need one
            var client = ServiceLocator.Instance.Resolve<IAzureClient>()?.Client;

            if (!string.IsNullOrWhiteSpace(Settings.Current.AuthToken) &&
                !string.IsNullOrWhiteSpace(Settings.Current.AzureMobileUserId))
            {
                client.CurrentUser = new MobileServiceUser(Settings.Current.AzureMobileUserId)
                {
                    MobileServiceAuthenticationToken = Settings.Current.AuthToken
                };
            }

            var path = $"syncstore{Settings.Current.DatabaseId}.db";
            //setup our local sqlite store and intialize our table
            store = new MobileServiceSQLiteStore(path);

            store.DefineTable<UserProfile>();
            store.DefineTable<TripPoint>();
            store.DefineTable<Photo>();
            store.DefineTable<Trip>();
            store.DefineTable<POI>();
            store.DefineTable<IOTHubData>();

            //await client.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());
            await client.SyncContext.InitializeAsync(store, new AzureMobileClientHandler());

            IsInitialized = true;
        }

        public async Task<bool> SyncAllAsync(bool syncUserSpecific)
        {
            if (!IsInitialized)
                await InitializeAsync();

            var taskList = new List<Task<bool>> {TripStore.SyncAsync()};

            var successes = await Task.WhenAll(taskList);
            return successes.Any(x => !x); //if any were a failure.
        }

        public async Task DropEverythingAsync()
        {
            Settings.Current.UpdateDatabaseId();
            await TripStore.DropTable();
            await PhotoStore.DropTable();
            await UserStore.DropTable();
            await IOTHubStore.DropTable();
            IsInitialized = false;
            await InitializeAsync();
        }

        public bool IsInitialized { get; private set; }

        ITripStore tripStore;
        public ITripStore TripStore => tripStore ?? (tripStore = ServiceLocator.Instance.Resolve<ITripStore>());

        IPhotoStore photoStore;
        public IPhotoStore PhotoStore => photoStore ?? (photoStore = ServiceLocator.Instance.Resolve<IPhotoStore>());

        IUserStore userStore;
        public IUserStore UserStore => userStore ?? (userStore = ServiceLocator.Instance.Resolve<IUserStore>());

        ITripPointStore tripPointStore;
        public ITripPointStore TripPointStore => tripPointStore ?? (tripPointStore = ServiceLocator.Instance.Resolve<ITripPointStore>());

        IHubIOTStore iotHubStore;

        public IHubIOTStore IOTHubStore
            => iotHubStore ?? (iotHubStore = ServiceLocator.Instance.Resolve<IHubIOTStore>());

        IPOIStore poiStore;
        public IPOIStore POIStore => poiStore ?? (poiStore = ServiceLocator.Instance.Resolve<IPOIStore>());

        #endregion
    }
}