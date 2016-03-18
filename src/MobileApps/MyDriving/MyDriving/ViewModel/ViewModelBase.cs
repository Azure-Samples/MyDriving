using MvvmHelpers;
using MyDriving.Utils;
using MyDriving.DataStore.Abstractions;

using MyDriving.AzureClient;

namespace MyDriving.ViewModel
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
        }

        public Settings Settings
        {
            get { return Settings.Current; }
        }

        static IStoreManager storeManager;
        public static IStoreManager StoreManager => storeManager ?? (storeManager = ServiceLocator.Instance.Resolve<IStoreManager>()); 
    }
}
