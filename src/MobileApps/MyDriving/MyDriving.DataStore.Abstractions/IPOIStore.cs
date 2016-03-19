using MyDriving.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDriving.DataStore.Abstractions
{
    public interface IPOIStore : IBaseStore<POI>
    {
        Task<IEnumerable<POI>> GetItemsAsync(string tripId);


    }
}
