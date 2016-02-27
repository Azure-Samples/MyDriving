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

        public async Task<Photo> GetTripMainPhoto(string tripId)
        {
            var items = await GetTripPhotos(tripId);

            if (items.Count() > 0)
                return items.ElementAt(0);

            return null;
        }

        public async Task<IEnumerable<Photo>> GetTripPhotos(string tripId)
        {
            return await Table.Where(s => s.TripId == tripId).ToEnumerableAsync();
        }
    }
}

