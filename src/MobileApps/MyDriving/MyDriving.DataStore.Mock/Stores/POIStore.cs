// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using MyDriving.DataObjects;
using MyDriving.DataStore.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyDriving.DataStore.Mock.Stores
{
    public class POIStore : BaseStore<POI>, IPOIStore
    {
        public override Task<bool> PullLatestAsync()
        {
            return Task.FromResult(true);
        }

        public override Task<bool> SyncAsync()
        {
            return Task.FromResult(true);
        }

        public override Task<IEnumerable<POI>> GetItemsAsync(int skip = 0, int take = 100, bool forceRefresh = false)
        {
            var poiList = GenRandomPOI();
            return Task.FromResult(poiList.AsEnumerable());
        }

        public async Task<IEnumerable<POI>> GetItemsAsync(string tripId)
        {
            return new List<POI>();
        }

        private List<POI> GenRandomPOI()
        {
            var poiList = new List<POI>();

            Random r = new Random();
            int numPoints = r.Next(1, 25);
            for (int i = 0; i < numPoints; i++)
            {
                POI p = AddMockPOI(r);
                poiList.Add(p);
            }

            return poiList;
        }

        private POI AddMockPOI(Random r)
        {
            var p = new POI
            {
                Longitude = r.NextDouble()*r.Next(-180, 181),
                Latitude = r.NextDouble()*r.Next(-90, 91),
                Timestamp = DateTime.Today,
                POIType = (r.Next(1, 3) == 1) ? POIType.HardAcceleration : POIType.HardBrake
            };

            return p;
        }
    }
}