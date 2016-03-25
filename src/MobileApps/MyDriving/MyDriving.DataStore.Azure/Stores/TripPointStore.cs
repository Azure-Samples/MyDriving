using Microsoft.WindowsAzure.MobileServices;
using MyDriving.DataObjects;
using MyDriving.DataStore.Abstractions;
using MyDriving.Utils;
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

            try
            {
                var pullId = $"{id}";
                await Table.PullAsync(pullId, Table.Where(s => s.TripId == id)).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                Logger.Instance.WriteLine("Unable to pull items, that is alright as we have offline capabilities: " + ex);
            }

            return await Table.Where(s => s.TripId == id).OrderBy(p => p.Sequence).ToEnumerableAsync().ConfigureAwait(false);
        }
    }
}
