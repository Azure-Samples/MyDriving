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

namespace MyDriving.DataStore.Azure
{
    public class StoreManager : IStoreManager
    {
        #region IStoreManager implementation

        MobileServiceSQLiteStore _store;

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
            _store = new MobileServiceSQLiteStore(path);

            _store.DefineTable<UserProfile>();
            _store.DefineTable<TripPoint>();
            _store.DefineTable<Photo>();
            _store.DefineTable<Trip>();
            _store.DefineTable<IOTHubData>();

            await client.SyncContext.InitializeAsync(_store, new MobileServiceSyncHandler()).ConfigureAwait(false);

            IsInitialized = true;
        }

        public async Task<bool> SyncAllAsync(bool syncUserSpecific)
        {
            if (!IsInitialized)
                await InitializeAsync();

            var taskList = new List<Task<bool>> {TripStore.SyncAsync()};

            var successes = await Task.WhenAll(taskList).ConfigureAwait(false);
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

        ITripStore _tripStore;
        public ITripStore TripStore => _tripStore ?? (_tripStore = ServiceLocator.Instance.Resolve<ITripStore>());

        IPhotoStore _photoStore;
        public IPhotoStore PhotoStore => _photoStore ?? (_photoStore = ServiceLocator.Instance.Resolve<IPhotoStore>());

        IUserStore _userStore;
        public IUserStore UserStore => _userStore ?? (_userStore = ServiceLocator.Instance.Resolve<IUserStore>());

        IHubIOTStore _iotHubStore;

        public IHubIOTStore IOTHubStore
            => _iotHubStore ?? (_iotHubStore = ServiceLocator.Instance.Resolve<IHubIOTStore>());

        #endregion
    }
}