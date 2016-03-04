﻿using MyTrips.DataObjects;
using MyTrips.DataStore.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrips.DataStore.Mock.Stores
{
    public class IOTHubStore : BaseStore<IOTHubData>, IHubIOTStore
    {
        List<IOTHubData> iotHubData;

        public IOTHubStore()
        {
            this.iotHubData = new List<IOTHubData>();
        }

        public override Task<bool> PullLatestAsync()
        {
            return Task.FromResult(true);
        }

        public override Task<bool> SyncAsync()
        {
            return Task.FromResult(true);
        }

        public override Task<IEnumerable<IOTHubData>> GetItemsAsync(int skip = 0, int take = 100, bool forceRefresh = false)
        {
            return Task.FromResult(this.iotHubData.AsEnumerable());
        }

        public override Task<bool> InsertAsync(IOTHubData item)
        {
            this.iotHubData.Add(item);
            return Task.FromResult(true);
        }

        public override Task<bool> RemoveAsync(IOTHubData item)
        {
            var dataItem = this.iotHubData.Where(i => i.Id == item.Id).First();
            this.iotHubData.Remove(dataItem);
            return Task.FromResult(true);
        }

        public override Task<bool> DropTable()
        {
            this.iotHubData.Clear();
            return Task.FromResult(true);
        }
    }
}
