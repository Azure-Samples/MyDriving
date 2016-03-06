using System;
using MyTrips.DataStore.Abstractions;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using MyTrips.Utils;
using MyTrips.DataObjects;
using System.Collections.Generic;
using System.Linq;
using MyTrips.AzureClient;

namespace MyTrips.DataStore.Azure
{
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

            if (!string.IsNullOrWhiteSpace (Settings.Current.AuthToken) && !string.IsNullOrWhiteSpace (Settings.Current.UserId)) {
                client.CurrentUser = new MobileServiceUser (Settings.Current.UserId);
                client.CurrentUser.MobileServiceAuthenticationToken = Settings.Current.AuthToken;
            }
            
            var path = $"syncstore{Settings.Current.DatabaseId}.db";
            //setup our local sqlite store and intialize our table
            store = new MobileServiceSQLiteStore(path);

            store.DefineTable<UserProfile>();
            store.DefineTable<Tip>();
            store.DefineTable<TripPoint>();
            store.DefineTable<Photo>();
            store.DefineTable<Trip>();
            store.DefineTable<IOTHubData>();

            await client.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler()).ConfigureAwait(false);

            IsInitialized = true;
        }

        public async Task<bool> SyncAllAsync(bool syncUserSpecific)
        {
            if(!IsInitialized)
                await InitializeAsync();

            var taskList = new List<Task<bool>>();
            taskList.Add(TripStore.SyncAsync());

            var successes = await Task.WhenAll(taskList).ConfigureAwait(false);
            return successes.Any(x => !x);//if any were a failure.
        }

        public Task DropEverythingAsync()
        {
            Settings.Current.UpdateDatabaseId();
            TripStore.DropTable();
            IsInitialized = false;
            return Task.FromResult(true);
        }

        public bool IsInitialized
        {
            get;
            private set;
        }

        ITripStore tripStore;
        public ITripStore TripStore => tripStore ?? (tripStore = ServiceLocator.Instance.Resolve<ITripStore>());

        IPhotoStore photoStore;
        public IPhotoStore PhotoStore => photoStore ?? (photoStore = ServiceLocator.Instance.Resolve<IPhotoStore>());

        IUserStore userStore;
        public IUserStore UserStore => userStore ?? (userStore = ServiceLocator.Instance.Resolve<IUserStore>());

        IHubIOTStore iotHubStore;
        public IHubIOTStore IOTHubStore => iotHubStore ?? (iotHubStore = ServiceLocator.Instance.Resolve<IHubIOTStore>());

        #endregion
    }
}

