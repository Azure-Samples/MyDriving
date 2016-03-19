﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using MvvmHelpers;
using MyDriving.Utils;
using MyDriving.DataStore.Abstractions;
using MyDriving.AzureClient;

namespace MyDriving.ViewModel
{
    public class ViewModelBase : BaseViewModel
    {
        static IStoreManager _storeManager;

        public Settings Settings => Settings.Current;

        public static IStoreManager StoreManager
            => _storeManager ?? (_storeManager = ServiceLocator.Instance.Resolve<IStoreManager>());

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

                //TODO: Always use the mock POI store until the ML service is available where we can switch to the real one
                //When real ML service is available, uncomment the azure based store and remove mock
                //ServiceLocator.Instance.Add<IPOIStore, DataStore.Azure.Stores.POIStore>();
                ServiceLocator.Instance.Add<IPOIStore, DataStore.Mock.Stores.POIStore>();
            }
        }
    }
}