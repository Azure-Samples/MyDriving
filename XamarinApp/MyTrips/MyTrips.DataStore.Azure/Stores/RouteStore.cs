using System;
using MyTrips.DataObjects;
using MyTrips.DataStore.Abstractions;

namespace MyTrips.DataStore.Azure.Stores
{
    public class RouteStore : BaseStore<Route>, IRouteStore
    {
        public RouteStore()
        {
        }
    }
}

