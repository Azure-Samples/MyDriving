using MvvmHelpers;
using MyTrips.Utils;
using MyTrips.DataStore.Abstractions;

//Use Mock
using MyTrips.DataStore.Mock;
using MyTrips.DataStore.Mock.Stores;

//Use Azure
//using MyTrips.DataStore.Azure;
//using MyTrips.DataStore.Azure.Stores;

namespace MyTrips.ViewModel
{
    public class ViewModelBase : BaseViewModel
    {
        
        public static void Init()
        {
            ServiceLocator.Instance.Add<ITripStore, TripStore>();
            ServiceLocator.Instance.Add<IStoreManager, StoreManager>();

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
