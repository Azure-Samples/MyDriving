using MyTrips.AzureClient;
using MyTrips.DataObjects;
using MyTrips.DataStore.Abstractions;
using MyTrips.Utils;
using Newtonsoft.Json.Linq;
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

        

        /*public override async Task<bool> RemoveAsync(IOTHubData item)
        {
            var client = ServiceLocator.Instance.Resolve<IAzureClient>()?.Client;
            await client.SyncContext.Store.DeleteAsync(nameof(IOTHubData), new[] { item.Id });
            return true;
        }

        public override async Task<bool> InsertAsync(IOTHubData item)
        {
            var client = ServiceLocator.Instance.Resolve<IAzureClient>()?.Client;
            await client.SyncContext.Store.UpsertAsync(nameof(IOTHubData), new[] { JObject.FromObject(item) }, true);

            return true;
        }
        

        public override async Task<IOTHubData> GetItemAsync(string id)
        {
            var client = ServiceLocator.Instance.Resolve<IAzureClient>()?.Client;
            var item = await client.SyncContext.Store.LookupAsync(nameof(IOTHubData), id);
            if (item == null)
                return null;

            return item.ToObject<IOTHubData>();
        }*/

        public override Task<bool> DropTable()
        {
            return Task.FromResult(true);
        }
    }
}
