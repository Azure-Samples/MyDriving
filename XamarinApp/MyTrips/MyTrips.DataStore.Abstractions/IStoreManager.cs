using System;
using System.Threading.Tasks;

namespace MyTrips.DataStore.Abstractions
{
    public interface IStoreManager 
    {
        bool IsInitialized {get;}
        Task InitializeAsync();
        ITripStore TripStore { get; }
        IPhotoStore PhotoStore { get; }
        IUserStore UserStore { get; }
        IHubIOTStore IOTHubStore { get; }
        Task<bool> SyncAllAsync(bool syncUserSpecific);
        Task DropEverythingAsync();
    }
}

