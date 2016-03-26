// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Threading.Tasks;

namespace MyDriving.DataStore.Abstractions
{
    public interface IStoreManager
    {
        bool IsInitialized { get; }
        ITripStore TripStore { get; }
        IPhotoStore PhotoStore { get; }
        IUserStore UserStore { get; }
        IHubIOTStore IOTHubStore { get; }
        IPOIStore POIStore { get; }
        ITripPointStore TripPointStore { get; }
        Task<bool> SyncAllAsync(bool syncUserSpecific);
        Task DropEverythingAsync();
        Task InitializeAsync();
    }
}