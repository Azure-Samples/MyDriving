// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyDriving.DataStore.Abstractions
{
    public interface IBaseStore<T>
    {
        string Identifier { get; }
        Task InitializeStoreAsync();
        Task<IEnumerable<T>> GetItemsAsync(int skip = 0, int take = 100, bool forceRefresh = false);
        Task<T> GetItemAsync(string id);
        Task<bool> InsertAsync(T item);
        Task<bool> UpdateAsync(T item);
        Task<bool> RemoveAsync(T item);
        Task<bool> RemoveItemsAsync(IEnumerable<T> items);
        Task<bool> SyncAsync();
        Task<bool> DropTable();
    }
}