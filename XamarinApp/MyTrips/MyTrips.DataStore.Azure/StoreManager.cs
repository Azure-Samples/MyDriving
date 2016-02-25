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

namespace MyTrips.DataStore.Azure
{
    public class StoreManager : IStoreManager
    {
        public static MobileServiceClient MobileService { get; set; }



        #region IStoreManager implementation

        public async Task InitializeAsync()
        {
            if (IsInitialized)
                return;
            var handler = new AuthHandler();

            //Create our client
            MobileService = new MobileServiceClient("https://smarttrips.azurewebsites.net");

            handler.Client = MobileService;

            if (!string.IsNullOrWhiteSpace (Settings.Current.AuthToken) && !string.IsNullOrWhiteSpace (Settings.Current.UserId)) {
                MobileService.CurrentUser = new MobileServiceUser (Settings.Current.UserId);
                MobileService.CurrentUser.MobileServiceAuthenticationToken = Settings.Current.AuthToken;
            }

            if (handler != null)
                handler.Client = MobileService;
            
            var path = $"syncstore{Settings.Current.DatabaseId}.db";
            //setup our local sqlite store and intialize our table
            var store = new MobileServiceSQLiteStore(path);

            store.DefineTable<Feedback>();
            store.DefineTable<Route>();
            store.DefineTable<Telemetry>();
            store.DefineTable<Trail>();
            store.DefineTable<Trip>();

            await MobileService.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler()).ConfigureAwait(false);


            IsInitialized = true;
        }

        public async Task<bool> SyncAllAsync(bool syncUserSpecific)
        {

            if(!IsInitialized)
                await InitializeAsync();

            var taskList = new List<Task<bool>>();
            taskList.Add(TripStore.SyncAsync());
            taskList.Add(FeedbackStore.SyncAsync());
            taskList.Add(RouteStore.SyncAsync());


            var successes = await Task.WhenAll(taskList).ConfigureAwait(false);
            return successes.Any(x => !x);//if any were a failure.
        }

        public Task DropEverythingAsync()
        {
            Settings.Current.UpdateDatabaseId();
            TripStore.DropTable();
            FeedbackStore.DropTable();
            RouteStore.DropTable();
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

        IFeedbackStore feedbackStore;
        public IFeedbackStore FeedbackStore => feedbackStore ?? (feedbackStore = ServiceLocator.Instance.Resolve<IFeedbackStore>());

        IRouteStore routeStore;
        public IRouteStore RouteStore => routeStore ?? (routeStore = ServiceLocator.Instance.Resolve<IRouteStore>());


        #endregion
    }
}

