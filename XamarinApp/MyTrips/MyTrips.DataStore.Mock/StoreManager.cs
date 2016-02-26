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

        #endregion
    }
}

