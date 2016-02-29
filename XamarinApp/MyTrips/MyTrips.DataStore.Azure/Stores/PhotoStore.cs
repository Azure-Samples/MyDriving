using System;
using MyTrips.DataObjects;
using MyTrips.DataStore.Abstractions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace MyTrips.DataStore.Azure.Stores
{
    public class PhotoStore : BaseStore<Photo>, IPhotoStore
    {
        public override Task<bool> PullLatestAsync()
        {
            return Task.FromResult(true);
        }

        public override Task<bool> SyncAsync()
        {
            return Task.FromResult(true);
        }

        public Task<IEnumerable<Photo>> GetTripPhotos(string tripId)
        {
            return Table.Where(s => s.TripId == tripId).ToEnumerableAsync();
        }
    }
}

