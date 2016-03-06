using MyTrips.DataObjects;
using MyTrips.DataStore.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrips.DataStore.Azure.Stores
{
    public class IOTHubStore : BaseStore<IOTHubData>, IHubIOTStore
    {
        public override Task<bool> PullLatestAsync()
        {
            return Task.FromResult(true);
        }

        public override Task<bool> SyncAsync()
        {
            return Task.FromResult(true);
        }

        public override Task<bool> DropTable()
        {
            //TODO: need to figure out how to drop table or delete contents from table (currently exception thrown)
            throw new NotImplementedException();
        }
    }
}
