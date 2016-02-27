using System;
using MyTrips.DataObjects;
using MyTrips.DataStore.Abstractions;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MyTrips.DataStore.Mock.Stores
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
            return null;
        }

        public async Task<IEnumerable<Photo>> GetTripPhotos(string tripId)
        {
            return new List<Photo>();
        }

    }
}

