using System;
using MyTrips.DataStore.Abstractions;
using System.Threading.Tasks;

namespace MyTrips.DataStore.Azure.Stores
{
    public class BaseStore<T> : IBaseStore<T>
    {
        #region IBaseStore implementation
        public Task InitializeStoreAsync()
        {
            throw new NotImplementedException();
        }
        public Task<System.Collections.Generic.IEnumerable<T>> GetItemsAsync(bool forceRefresh = false)
        {
            throw new NotImplementedException();
        }
        public Task<T> GetItemAsync(string id)
        {
            throw new NotImplementedException();
        }
        public Task<bool> InsertAsync(T item)
        {
            throw new NotImplementedException();
        }
        public Task<bool> UpdateAsync(T item)
        {
            throw new NotImplementedException();
        }
        public Task<bool> RemoveAsync(T item)
        {
            throw new NotImplementedException();
        }
        public Task<bool> SyncAsync()
        {
            return Task.FromResult(true);
        }
        public void DropTable()
        {
            throw new NotImplementedException();
        }
        public string Identifier => "store";
        #endregion
       
    }
}

