using System;
using MyTrips.DataStore.Abstractions;

namespace MyTrips.DataStore.Azure
{
    public class StoreManager : IStoreManager
    {
        public StoreManager()
        {
        }

        #region IStoreManager implementation

        public System.Threading.Tasks.Task InitializeAsync()
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<bool> SyncAllAsync(bool syncUserSpecific)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task DropEverythingAsync()
        {
            throw new NotImplementedException();
        }

        public bool IsInitialized
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ITripStore TripStore
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IFeedbackStore FeedbackStore
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IRouteStore RouteStore
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}

