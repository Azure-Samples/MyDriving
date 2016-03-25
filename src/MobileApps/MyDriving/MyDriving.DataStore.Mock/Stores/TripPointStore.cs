using MyDriving.DataObjects;
using MyDriving.DataStore.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDriving.DataStore.Mock.Stores
{
    public class TripPointStore : BaseStore<TripPoint>, ITripPointStore
    {
        public async Task<IEnumerable<TripPoint>> GetPointsForTripAsync(string id)
        {
            return new List<TripPoint>();
        }
    }
}
