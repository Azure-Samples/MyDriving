using MyDriving.DataObjects;
using MyDriving.DataStore.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDriving.DataStore.Azure.Stores
{
    public class POIStore : BaseStore<POI>, IPOIStore
    {
        public async Task<IEnumerable<POI>> GetItemsAsync(string tripId)
        {
            return new List<POI>();
        }
    }
}
