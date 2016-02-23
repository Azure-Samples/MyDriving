using System;
using MyTrips.DataObjects;
using MyTrips.DataStore.Abstractions;
using System.Threading.Tasks;
using System.Reflection;
using Plugin.EmbeddedResource;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace MyTrips.DataStore.Mock.Stores
{
    public class TripStore : BaseStore<Trip>, ITripStore
    {
        bool initialized;
        List<Trip> Trips {get;set;}

        public override Task InitializeStoreAsync()
        {
            if (initialized)
                return Task.FromResult(true);

            initialized = true;
            
            Trips = new List<Trip>();
            var json = ResourceLoader.GetEmbeddedResourceString(Assembly.Load(new AssemblyName("MyTrips.DataStore.Mock")), "sampletrip.json");

            for(int i = 0; i < 5; i++)
            {
                var trip = JsonConvert.DeserializeObject<Trip>(json);
                trip.Name += " #" + i.ToString();
                Trips.Add(trip);
            }

            return Task.FromResult(true);
        }

        public override async Task<IEnumerable<Trip>> GetItemsAsync(bool forceRefresh = false)
        {
            if (!initialized)
                await InitializeStoreAsync();
            
            return Trips;
        }

        public override async Task<Trip> GetItemAsync(string id)
        {
            if (!initialized)
                await InitializeStoreAsync();

            return Trips[0];
        }
    }
}

