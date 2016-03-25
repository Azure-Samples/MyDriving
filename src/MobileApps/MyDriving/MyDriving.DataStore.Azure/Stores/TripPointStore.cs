using MyDriving.DataObjects;
using MyDriving.DataStore.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDriving.DataStore.Azure.Stores
{
    public class TripPointStore : BaseStore<TripPoint>, ITripPointStore
    {
        public override string Identifier => "TripPoint";

        public async Task<IEnumerable<TripPoint>> GetPointsForTripAsync(string id)
        {
            await InitializeStoreAsync().ConfigureAwait(false);
            
            await base.SyncAsync().ConfigureAwait(false);

            return await Table.Where(s => s.TripId == id).OrderBy(p => p.Sequence).ToEnumerableAsync().ConfigureAwait(false);
        }
    }
}
