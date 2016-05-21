﻿// Copyright (c) Microsoft Corporation. All rights reserved.
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
        readonly ITripPointStore pointStore;
        public TripStore()
        {
            photoStore = ServiceLocator.Instance.Resolve<IPhotoStore>();
            pointStore = ServiceLocator.Instance.Resolve<ITripPointStore>();
        }

        public override string Identifier => "Trip";

        public override async Task<bool> InsertAsync(Trip item)
        {
            /*foreach (var point in item.Points)
            {
                await pointStore.InsertAsync(point);
            }
            await pointStore.SyncAsync();*/
            return await base.InsertAsync(item);
 
        }

        public override async Task<IEnumerable<Trip>> GetItemsAsync(int skip = 0, int take = 100,
            bool forceRefresh = false)
        {
            await InitializeStoreAsync().ConfigureAwait(false);
            if (forceRefresh)
            {
                await SyncAsync().ConfigureAwait(false);
            }

            var items = await Table.Skip(skip).Take(take).OrderByDescending(s => s.RecordedTimeStamp).ToEnumerableAsync().ConfigureAwait(false);

            foreach (var item in items)
            {
                item.Photos = new List<Photo>();
                var photos = await photoStore.GetTripPhotos(item.Id).ConfigureAwait(false);
                foreach (var photo in photos)
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

                var points = item.Points;
                if (points == null || points.Count == 0)
                {
                    points = new List<TripPoint>(await pointStore.GetPointsForTripAsync(item.Id));
                }

                foreach (var point in points)
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
                Logger.Instance.Track($"Unable to remove item {item.Id}:{e}");
            }

            return result;
        }
    }
}