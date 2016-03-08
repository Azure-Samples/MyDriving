using MvvmHelpers;
using MyTrips.Utils;
using MyTrips.DataStore.Abstractions;

using MyTrips.AzureClient;

namespace MyTrips.ViewModel
{
    public class ViewModelBase : BaseViewModel
    {
        
        public static void Init(bool useMock = false)
        {
            ServiceLocator.Instance.Add<IAzureClient, AzureClient.AzureClient>();
            if (useMock)
            {
                
                ServiceLocator.Instance.Add<ITripStore, DataStore.Mock.Stores.TripStore>();
                ServiceLocator.Instance.Add<IPhotoStore, DataStore.Mock.Stores.PhotoStore>();
                ServiceLocator.Instance.Add<IUserStore, DataStore.Mock.Stores.UserStore>();
                ServiceLocator.Instance.Add<IHubIOTStore, DataStore.Mock.Stores.IOTHubStore>();
                ServiceLocator.Instance.Add<IStoreManager, DataStore.Mock.StoreManager>();
            }
            else
            {
                ServiceLocator.Instance.Add<ITripStore, DataStore.Azure.Stores.TripStore>();
                ServiceLocator.Instance.Add<IPhotoStore, DataStore.Azure.Stores.PhotoStore>();
                ServiceLocator.Instance.Add<IUserStore, DataStore.Azure.Stores.UserStore>();
                ServiceLocator.Instance.Add<IHubIOTStore, DataStore.Azure.Stores.IOTHubStore>();
                ServiceLocator.Instance.Add<IStoreManager, DataStore.Azure.StoreManager>();
            }

            //TODO: Put this somewhere....
            ServiceLocator.Instance.Resolve<IStoreManager>().InitializeAsync();

        }

        public Settings Settings
        {
            get { return Settings.Current; }
        }

        IStoreManager storeManager;
        public IStoreManager StoreManager => storeManager ?? (storeManager = ServiceLocator.Instance.Resolve<IStoreManager>()); 

    }

}
