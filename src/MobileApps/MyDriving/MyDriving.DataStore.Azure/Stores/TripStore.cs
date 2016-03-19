// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using MyDriving.DataObjects;
using MyDriving.DataStore.Abstractions;
using System.Threading.Tasks;
using MyDriving.Utils;
using System.Collections.Generic;
using System.Linq;
using MyDriving.AzureClient;

namespace MyDriving.DataStore.Azure.Stores
{
    public class TripStore : BaseStore<Trip>, ITripStore
    {
        readonly IPhotoStore photoStore;

        public TripStore()
        {
            photoStore = ServiceLocator.Instance.Resolve<IPhotoStore>();
        }

        public override string Identifier => "Trip";

        public override async Task<IEnumerable<Trip>> GetItemsAsync(int skip = 0, int take = 100,
            bool forceRefresh = false)
        {
            var items = await base.GetItemsAsync(skip, take, forceRefresh).ConfigureAwait(false);
            foreach (var item in items)
            {
                item.Photos = new List<Photo>();
                var photos = await photoStore.GetTripPhotos(item.Id).ConfigureAwait(false);
                foreach (var photo in photos)
                    item.Photos.Add(photo);
            }

            return items.OrderByDescending(s => s.RecordedTimeStamp);
        }

        public override async Task<Trip> GetItemAsync(string id)
        {
            var item = await base.GetItemAsync(id);

            if (item.Photos == null)
                item.Photos = new List<Photo>();
            else
                item.Photos.Clear();

            var photos = await photoStore.GetTripPhotos(item.Id).ConfigureAwait(false);
            foreach (var photo in photos)
                item.Photos.Add(photo);


            item.Points = item.Points.OrderBy(p => p.Sequence).ToArray();

            return item;
        }

        public override async Task<bool> RemoveAsync(Trip item)
        {
            bool result = false;
            try
            {
                await InitializeStoreAsync().ConfigureAwait(false);

                var t = ServiceLocator.Instance.Resolve<IAzureClient>()?.Client?.GetSyncTable<TripPoint>();
                foreach (var point in item.Points)
                {
                    await t.DeleteAsync(point).ConfigureAwait(false);
                }


                await PullLatestAsync().ConfigureAwait(false);
                await Table.DeleteAsync(item).ConfigureAwait(false);
                await SyncAsync().ConfigureAwait(false);
                result = true;
            }
            catch (Exception e)
            {
                Logger.Instance.WriteLine($"Unable to remove item {item.Id}:{e}");
            }

            return result;
        }
    }
}