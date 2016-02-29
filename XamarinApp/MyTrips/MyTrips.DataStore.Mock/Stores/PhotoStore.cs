using System;
using MyTrips.DataObjects;
using MyTrips.DataStore.Abstractions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace MyTrips.DataStore.Mock.Stores
{
    public class PhotoStore : BaseStore<Photo>, IPhotoStore
    {
        List<Photo> photos;
        public PhotoStore()
        {
            photos = new List<Photo>();
        }
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
            return Task.FromResult(photos.Where(a => a.TripId == tripId));
        }

        public override Task<bool> InsertAsync(Photo item)
        {
            photos.Add(item);
            return Task.FromResult(true);
        }

    }
}

