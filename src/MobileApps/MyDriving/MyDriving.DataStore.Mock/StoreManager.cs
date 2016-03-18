// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using MyDriving.DataStore.Abstractions;
using MyDriving.Utils;
using System.Threading.Tasks;

namespace MyDriving.DataStore.Mock
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

        ITripStore _tripStore;
        public ITripStore TripStore => _tripStore ?? (_tripStore = ServiceLocator.Instance.Resolve<ITripStore>());

        IPhotoStore _photoStore;
        public IPhotoStore PhotoStore => _photoStore ?? (_photoStore = ServiceLocator.Instance.Resolve<IPhotoStore>());

        IUserStore _userStore;
        public IUserStore UserStore => _userStore ?? (_userStore = ServiceLocator.Instance.Resolve<IUserStore>());

        IHubIOTStore _iotHubStore;

        public IHubIOTStore IOTHubStore
            => _iotHubStore ?? (_iotHubStore = ServiceLocator.Instance.Resolve<IHubIOTStore>());

        IPOIStore poiStore;
        public IPOIStore POIStore => poiStore ?? (poiStore = ServiceLocator.Instance.Resolve<IPOIStore>());

        #endregion
    }
}