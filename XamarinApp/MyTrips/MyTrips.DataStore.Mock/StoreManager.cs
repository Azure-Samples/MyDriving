using System;
using MyTrips.DataStore.Abstractions;
using MyTrips.Utils;
using System.Threading.Tasks;

namespace MyTrips.DataStore.Mock
{
    public class StoreManager : IStoreManager
    {
        

        #region IStoreManager implementation

        public async Task InitializeAsync()
        {
            await TripStore.InitializeStoreAsync();
            await FeedbackStore.InitializeStoreAsync();
            await RouteStore.InitializeStoreAsync();
        }

        public Task<bool> SyncAllAsync(bool syncUserSpecific)
        {
            return Task.FromResult(true);
        }

        public Task DropEverythingAsync()
        {
            return Task.FromResult(true);
        }

        public bool IsInitialized => true;

        ITripStore tripStore;
        public ITripStore TripStore => tripStore ?? (tripStore = ServiceLocator.Instance.Resolve<ITripStore>());

        IFeedbackStore feedbackStore;
        public IFeedbackStore FeedbackStore => feedbackStore ?? (feedbackStore = ServiceLocator.Instance.Resolve<IFeedbackStore>());

        IRouteStore routeStore;
        public IRouteStore RouteStore => routeStore ?? (routeStore = ServiceLocator.Instance.Resolve<IRouteStore>());

        #endregion
    }
}

