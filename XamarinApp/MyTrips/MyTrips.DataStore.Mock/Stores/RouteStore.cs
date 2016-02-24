using System;
using MyTrips.DataObjects;
using MyTrips.DataStore.Abstractions;
using System.Threading.Tasks;

namespace MyTrips.DataStore.Mock.Stores
{
    public class RouteStore : BaseStore<Route>, IRouteStore
    {
        public override Task InitializeStoreAsync()
        {
            return Task.FromResult(true);
        }
    }
}

