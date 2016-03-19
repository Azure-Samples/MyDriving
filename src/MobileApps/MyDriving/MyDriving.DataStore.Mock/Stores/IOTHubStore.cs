// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using MyDriving.DataObjects;
using MyDriving.DataStore.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyDriving.DataStore.Mock.Stores
{
    public class IOTHubStore : BaseStore<IOTHubData>, IHubIOTStore
    {
        readonly List<IOTHubData> iotHubData;

        public IOTHubStore()
        {
            iotHubData = new List<IOTHubData>();
        }

        public override Task<bool> PullLatestAsync()
        {
            return Task.FromResult(true);
        }

        public override Task<bool> SyncAsync()
        {
            return Task.FromResult(true);
        }

        public override Task<IEnumerable<IOTHubData>> GetItemsAsync(int skip = 0, int take = 100,
            bool forceRefresh = false)
        {
            return Task.FromResult(iotHubData.AsEnumerable());
        }

        public override Task<bool> InsertAsync(IOTHubData item)
        {
            iotHubData.Add(item);
            return Task.FromResult(true);
        }

        public override Task<bool> RemoveAsync(IOTHubData item)
        {
            var dataItem = iotHubData.First(i => i.Id == item.Id);
            iotHubData.Remove(dataItem);
            return Task.FromResult(true);
        }

        public override Task<bool> RemoveItemsAsync(IEnumerable<IOTHubData> items)
        {
            ((List<IOTHubData>) items).Clear();
            return Task.FromResult(true);
        }
    }
}