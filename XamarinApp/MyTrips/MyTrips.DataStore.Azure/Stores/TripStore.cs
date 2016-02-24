using System;
using MyTrips.DataObjects;
using MyTrips.DataStore.Abstractions;

namespace MyTrips.DataStore.Azure.Stores
{
    public class TripStore : BaseStore<Trip>, ITripStore
    {
        public TripStore()
        {
        }
    }
}

