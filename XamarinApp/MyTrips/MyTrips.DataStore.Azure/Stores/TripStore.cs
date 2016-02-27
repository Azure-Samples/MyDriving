using System;
using MyTrips.DataObjects;
using MyTrips.DataStore.Abstractions;
using System.Threading.Tasks;
using MyTrips.Utils;
using System.Collections.Generic;

namespace MyTrips.DataStore.Azure.Stores
{
    public class TripStore : BaseStore<Trip>, ITripStore
    {
        IPhotoStore photoStore;
        public TripStore()
        {
            photoStore = ServiceLocator.Instance.Resolve<IPhotoStore>();
        }

        public override async Task<IEnumerable<Trip>> GetItemsAsync(bool forceRefresh = false)
        {
            var items = await base.GetItemsAsync(forceRefresh);
            foreach (var item in items)
            {
                item.Photos = new List<Photo>();
                var photo = await photoStore.GetTripMainPhoto(item.Id);
                if (photo != null)
                    item.Photos.Add(photo);
            }

            return items;
        }

        public override async Task<Trip> GetItemAsync(string id)
        {
            var item = await base.GetItemAsync(id);

            if (item.Photos == null)
                item.Photos = new List<Photo>();
            else
                item.Photos.Clear();

            var photos = await photoStore.GetTripPhotos(item.Id);
            foreach(var photo in photos)
                item.Photos.Add(photo);

            return item;
        }
    }
}

