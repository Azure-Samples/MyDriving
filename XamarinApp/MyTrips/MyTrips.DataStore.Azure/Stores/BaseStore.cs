using System;
using MyTrips.DataStore.Abstractions;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using MyTrips.Utils;
using System.Collections.Generic;
using System.Linq;
using MyTrips.DataObjects;
using Plugin.Connectivity;
using System.Diagnostics;
using MyTrips.AzureClient;

namespace MyTrips.DataStore.Azure.Stores
{
    public class BaseStore<T> : IBaseStore<T> where T : class, IBaseDataObject, new()
    {
        IStoreManager storeManager;

        public virtual string Identifier => "Items";

        IMobileServiceSyncTable<T> table;
        protected IMobileServiceSyncTable<T> Table => 
            table ?? (table = ServiceLocator.Instance.Resolve<IAzureClient>()?.Client?.GetSyncTable<T>()); 

        public virtual Task<bool> DropTable()
        {
            table = null;
            return Task.FromResult(true);
        }

        public BaseStore()
        {

        }

        #region IBaseStore implementation

        public virtual async Task<bool> RemoveItemsAsync(IEnumerable<T> items)
        {
            bool result = true;
            foreach (var item in items)
            {
                result = result && await this.RemoveAsync(item);
            }

            return result;
        }


        public async Task InitializeStoreAsync()
        {
            if (storeManager == null)
                storeManager = ServiceLocator.Instance.Resolve<IStoreManager>();

            if (!storeManager.IsInitialized)
                await storeManager.InitializeAsync().ConfigureAwait(false);
        }

        public virtual async Task<IEnumerable<T>> GetItemsAsync(int skip = 0, int take = 100, bool forceRefresh = false)
        {
            await InitializeStoreAsync();
            if(forceRefresh)
                await PullLatestAsync();

            return await Table.ToEnumerableAsync().ConfigureAwait(false);
        }

        public virtual async Task<T> GetItemAsync(string id)
        {
            await InitializeStoreAsync().ConfigureAwait(false);
            await PullLatestAsync().ConfigureAwait(false);
            var item = await Table.LookupAsync(id).ConfigureAwait(false);
            return item;
        }

        public virtual async Task<bool> InsertAsync(T item)
        {
            await InitializeStoreAsync().ConfigureAwait(false);

            await Table.InsertAsync(item).ConfigureAwait(false);
            await SyncAsync();
            return true;
        }

        public virtual async Task<bool> UpdateAsync(T item)
        {
            await InitializeStoreAsync().ConfigureAwait(false);
            await Table.UpdateAsync(item).ConfigureAwait(false);
            await SyncAsync();
            return true;
        }

        public virtual async Task<bool> RemoveAsync(T item)
        {
            bool result = false;
            try
            {
               
                
                await InitializeStoreAsync().ConfigureAwait(false);
                await PullLatestAsync();
                await Table.DeleteAsync(item).ConfigureAwait(false);
                await SyncAsync();
                result = true;
            }
            catch (Exception e)
            {
                Logger.Instance.WriteLine(String.Format("Unable to remove item {0}:{1}", item.Id, e));
            }

            return result;
        }

        public virtual async Task<bool> PullLatestAsync()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                Logger.Instance.WriteLine("Unable to pull items, we are offline");
                return false;
            }
            try
            {
                await Table.PullAsync($"all{Identifier}", Table.CreateQuery()).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteLine("Unable to pull items, that is alright as we have offline capabilities: " + ex);
                return false;
            }
            return true;
        }


        public virtual async Task<bool> SyncAsync()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                Logger.Instance.WriteLine("Unable to sync items, we are offline");
                return false;
            }
            try
            {
                var client = ServiceLocator.Instance.Resolve<IAzureClient>()?.Client;
                if (client == null)
                {
                    Logger.Instance.WriteLine("Unable to sync items, client is null");

                    return false;
                }
                await PullLatestAsync().ConfigureAwait(false);
                await client.SyncContext.PushAsync().ConfigureAwait(false);

            }
            catch(Exception ex)
            {
                Logger.Instance.WriteLine("Unable to sync items, that is alright as we have offline capabilities: " + ex);
                return false;
            }
            finally
            {
            }
            return true;
        }
        #endregion
    }
}

