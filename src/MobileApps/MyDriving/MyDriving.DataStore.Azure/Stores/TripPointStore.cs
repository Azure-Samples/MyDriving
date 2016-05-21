﻿using Microsoft.WindowsAzure.MobileServices;
using MyDriving.AzureClient;
using MyDriving.DataObjects;
using MyDriving.DataStore.Abstractions;
using MyDriving.Utils;
using Plugin.Connectivity;
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

        public override async Task<bool> InsertAsync(TripPoint item)
        {
            await Table.InsertAsync(item).ConfigureAwait(false);
            return true;
        }

        public async Task<IEnumerable<TripPoint>> GetPointsForTripAsync(string id)
        {
            await InitializeStoreAsync().ConfigureAwait(false);

            //first look locally for points.
            var points = await Table.Where(s => s.TripId == id).OrderBy(p => p.Sequence).ToEnumerableAsync().ConfigureAwait(false);

            if (points.Any())
                return points;


            if (!CrossConnectivity.Current.IsConnected)
            {
                Logger.Instance.Track("Unable to pull items, we are offline");
                return new List<TripPoint>();
            }

            //if we don't have any points then we need to go up and pull them down.
            try
            {
                //await SyncAsync();
                var pullId = $"{id}";
                await Table.PullAsync(pullId, Table.Where(s => s.TripId == id)).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                Logger.Instance.Track("TripPointStore: Unable to pull items: " + ex.Message);
            }

            return await Table.Where(s => s.TripId == id).OrderBy(p => p.Sequence).ToEnumerableAsync().ConfigureAwait(false);
        }

        public override async Task<bool> SyncAsync()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                Logger.Instance.Track("Unable to sync items, we are offline");
                return false;
            }
            try
            {
                var client = ServiceLocator.Instance.Resolve<IAzureClient>()?.Client;
                await client.SyncContext.PushAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.Instance.Track("TripPointStore SyncAsync: Unable to sync items: " + ex.Message);
                return false;
            }

            return true;
        
        }
    }
}
