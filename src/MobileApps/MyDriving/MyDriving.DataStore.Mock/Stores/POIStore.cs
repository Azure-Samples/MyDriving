using MyDriving.DataObjects;
using MyDriving.DataStore.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDriving.DataStore.Mock.Stores
{
    public class POIStore : BaseStore<POI>, IPOIStore
    {
        List<POI> poiList;

        public POIStore()
        {
            this.poiList = new List<POI>();

            Random r = new Random();
            for (int i = 0; i < 25; i++)
            {
                POI p = AddMockPOI(r);
                poiList.Add(p);
            }
        }

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
            return Task.FromResult(this.poiList.AsEnumerable());
        }

        private POI AddMockPOI(Random r)
        {
            var p = new POI
            {
                Longitude = r.NextDouble() * r.Next(-180, 181),
                Latitude = r.NextDouble() * r.Next(-90, 91),
                Timestamp = DateTime.Today,
                POIType = (r.Next(1, 3) == 1) ? POIType.HardAcceleration : POIType.HardBrake
            };

            return p;
        }
    }
}
