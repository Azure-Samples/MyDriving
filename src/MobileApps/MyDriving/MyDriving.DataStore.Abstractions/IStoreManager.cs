using System;
using System.Threading.Tasks;

namespace MyDriving.DataStore.Abstractions
{
    public interface IStoreManager 
    {
        bool IsInitialized {get;}
        Task InitializeAsync();
        ITripStore TripStore { get; }
        IPhotoStore PhotoStore { get; }
        IUserStore UserStore { get; }
        IHubIOTStore IOTHubStore { get; }
        IPOIStore POIStore { get; }
        Task<bool> SyncAllAsync(bool syncUserSpecific);
        Task DropEverythingAsync();
    }
}

