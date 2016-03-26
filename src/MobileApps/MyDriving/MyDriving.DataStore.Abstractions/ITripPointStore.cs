using MyDriving.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDriving.DataStore.Abstractions
{
    public interface ITripPointStore : IBaseStore<TripPoint>
    {
        Task<IEnumerable<TripPoint>> GetPointsForTripAsync(string id);
    }
}
