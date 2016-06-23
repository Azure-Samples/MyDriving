// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using MyDriving.DataStore.Abstractions;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using MyDriving.Utils;
using System.Collections.Generic;
using MyDriving.DataObjects;
using Plugin.Connectivity;
using MyDriving.AzureClient;
using MyDriving.Utils.Interfaces;

namespace MyDriving.DataStore.Azure.Stores
{
    public class BaseStore<T> : IBaseStore<T> where T : class, IBaseDataObject, new()
    {
        IStoreManager storeManager;

        IMobileServiceSyncTable<T> table;

        protected IMobileServiceSyncTable<T> Table =>
            table ?? (table = ServiceLocator.Instance.Resolve<IAzureClient>()?.Client?.GetSyncTable<T>());

        public virtual string Identifier => "Items";

        public virtual Task<bool> DropTable()
        {
            table = null;
            return Task.FromResult(true);
        }

        #region IBaseStore implementation

        public virtual async Task<bool> RemoveItemsAsync(IEnumerable<T> items)
        {
            bool result = true;
            foreach (var item in items)
            {
                result = result && await RemoveAsync(item);
            }

            return result;
        }

        public async Task InitializeStoreAsync()
        {
            if (storeManager == null)
                storeManager = ServiceLocator.Instance.Resolve<IStoreManager>();

            if (!storeManager.IsInitialized)
                await storeManager.InitializeAsync();
        }

        public virtual async Task<IEnumerable<T>> GetItemsAsync(int skip = 0, int take = 100, bool forceRefresh = false)
        {
            await InitializeStoreAsync();
            if (forceRefresh)
            {
                await SyncAsync();
            }

            return await Table.ToEnumerableAsync();
        }

        public virtual async Task<T> GetItemAsync(string id)
        {
            await InitializeStoreAsync();
            await SyncAsync();
            var item = await Table.LookupAsync(id);
            return item;
        }

        public virtual async Task<bool> InsertAsync(T item)
        {
            await InitializeStoreAsync();
            await Table.InsertAsync(item); //Insert into the local store
            await SyncAsync(); //Send changes to the mobile service
            return true;
        }

        public virtual async Task<bool> UpdateAsync(T item)
        {
            await InitializeStoreAsync();
            await Table.UpdateAsync(item); //Delete from the local store
            await SyncAsync(); //Send changes to the mobile service
            return true;
        }

        public virtual async Task<bool> RemoveAsync(T item)
        {
            bool result = false;
            try
            {
                await InitializeStoreAsync();
                await Table.DeleteAsync(item); //Delete from the local store
                await SyncAsync(); //Send changes to the mobile service
                result = true;
            }
            catch (Exception e)
            {
                Logger.Instance.Track($"Unable to remove item {item.Id}:{e}");
            }

            return result;
        }

        //Note: do not call SyncAsync with ConfigureAwait(false) because when the authentication token expires,
        //the thread running this method needs to open the Login UI.
        //Also in the method which calls SyncAsync, do not use ConfigureAwait(false) before calling SyncAsync, because once ConfigureAwait(false) is used
        //in the context of an async method, the rest of that method's code may also run on a background thread.
        public virtual async Task<bool> SyncAsync()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                Logger.Instance.Track("Unable to sync items, we are offline");
                return false;
            }
            try
            {
                var client = ServiceLocator.Instance.Resolve<IAzureClient>()?.Client; 
                if (client == null)
                {
                    Logger.Instance.Track("Unable to sync items, client is null");

                    return false;
                }

                //push changes on the sync context before pulling new items
                await client.SyncContext.PushAsync();
                await Table.PullAsync($"all{Identifier}", Table.CreateQuery());
            }
            catch (Exception ex)
            {
                Logger.Instance.Track("SyncAsync: Unable to push/pull items: " + ex.Message);
                return false;
            }
            return true;
        }

        #endregion
    }
}