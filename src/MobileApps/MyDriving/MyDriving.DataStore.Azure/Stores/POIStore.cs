// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using MyDriving.DataObjects;
using MyDriving.DataStore.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MyDriving.DataStore.Azure.Stores
{
    public class POIStore : BaseStore<POI>, IPOIStore
    {
        public async Task<IEnumerable<POI>> GetItemsAsync(string tripId)
        {
            //Always force refresh
            await InitializeStoreAsync();
            await SyncAsync();
            return await Table.CreateQuery().Where(p => p.TripId == tripId).ToEnumerableAsync();
        }
    }
}