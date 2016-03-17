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
		List<Trip> Trips {get;set;} = new List<Trip> ();

        static Random random;
        IPhotoStore photoStore;
        static void AddTripDetails(Trip trip, int id, double lat, double lng, DateTime timestamp)
        {
            var pt = new TripPoint();
            pt.TripId = id.ToString();
            pt.Sequence = id;
            pt.Latitude = lat;
            pt.Longitude = lng;
            pt.RecordedTimeStamp = timestamp;
            pt.EngineLoad = random.Next(25, 75);
            pt.EngineFuelRate = random.Next(19, 25);
            pt.HasOBDData = true;
            trip.Points.Add(pt);
        }

        public static List<Trip> GetTrips()
        {
            random = new Random();
            Trip trip1 = new Trip();
            trip1.UserId = "Scott";
            trip1.Name = trip1.UserId + " - Redmond";
            trip1.Distance = 34;
            trip1.Photos = new List<Photo>();
            trip1.MainPhotoUrl = "http://cplinc.com/wp-content/uploads/2014/02/MS-1.jpg";
            var startTime = DateTime.UtcNow;
            trip1.RecordedTimeStamp = startTime;
            trip1.EndTimeStamp = startTime;
            var timeIncrement = 1;
            AddTripDetails(trip1, 1, 47.738928, -122.185165, startTime);
            AddTripDetails(trip1, 2, 47.738929, -122.185166, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip1, 3, 47.738930, -122.185167, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip1, 4, 47.738928, -122.185167, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip1, 5, 47.738926, -122.185166, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip1, 6, 47.738923, -122.185166, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip1, 7, 47.738924, -122.185166, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip1, 8, 47.738924, -122.185168, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip1, 9, 47.738925, -122.185168, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip1, 10, 47.738926, -122.185167, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip1, 11, 47.738927, -122.185167, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip1, 12, 47.738928, -122.185166, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip1, 13, 47.738929, -122.185166, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip1, 14, 47.738929, -122.185167, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip1, 15, 47.738939, -122.185169, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip1, 16, 47.738915, -122.185122, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip1, 17, 47.738377, -122.185458, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip1, 18, 47.738578, -122.185435, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip1, 19, 47.738579, -122.185436, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip1, 20, 47.739069, -122.185258, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip1, 21, 47.739172, -122.185237, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip1, 22, 47.738971, -122.185252, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip1, 23, 47.739356, -122.185168, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip1, 24, 47.738992, -122.185225, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip1, 25, 47.738988, -122.185227, startTime.AddMinutes(timeIncrement++));

            var trip2 = new Trip();
            trip2.UserId = "Cindy";
            trip2.Name = trip2.UserId + " - Seattle";
            trip2.Distance = 22;

            startTime = DateTime.UtcNow.AddDays(-2.0);
            trip2.RecordedTimeStamp = startTime;
            trip2.EndTimeStamp = startTime;
            timeIncrement = 1;
            AddTripDetails(trip2, 1, 47.738928, -122.185165, startTime);
            AddTripDetails(trip2, 2, 47.738929, -122.185166, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip2, 3, 47.738930, -122.185167, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip2, 4, 47.738928, -122.185167, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip2, 5, 47.738926, -122.185166, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip2, 6, 47.738923, -122.185166, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip2, 7, 47.738924, -122.185166, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip2, 8, 47.738924, -122.185168, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip2, 9, 47.738925, -122.185168, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip2, 10, 47.738926, -122.185167, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip2, 11, 47.738927, -122.185167, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip2, 12, 47.738928, -122.185166, startTime.AddMinutes(timeIncrement++));

            Trip trip3 = new Trip();
            trip3.UserId = "Hashi";
            trip3.Name = trip3.UserId + " - Portland";
            trip3.Distance = 173;

            startTime = DateTime.UtcNow.AddDays(-10.0);
            trip3.RecordedTimeStamp = startTime;
            trip3.EndTimeStamp = startTime;
            timeIncrement = 1;
            AddTripDetails(trip3, 2, 47.738929, -122.185166, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip3, 3, 47.738930, -122.185167, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip3, 4, 47.738928, -122.185167, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip3, 5, 47.738926, -122.185166, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip3, 6, 47.738923, -122.185166, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip3, 7, 47.738924, -122.185166, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip3, 8, 47.738924, -122.185168, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip3, 9, 47.738925, -122.185168, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip3, 10, 47.738926, -122.185167, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip3, 11, 47.738927, -122.185167, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip3, 12, 47.738928, -122.185166, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip3, 13, 47.738929, -122.185166, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip3, 14, 47.738929, -122.185167, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip3, 15, 47.738939, -122.185169, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip3, 16, 47.738915, -122.185122, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip3, 17, 47.738377, -122.185458, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip3, 18, 47.738578, -122.185435, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip3, 19, 47.738579, -122.185436, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip3, 20, 47.739069, -122.185258, startTime.AddMinutes(timeIncrement++));

            Trip trip4 = new Trip();
            trip4.UserId = "Scott";
            trip4.Name = trip3.UserId + " - Bellevue";
            trip4.Distance = 22;

            startTime = DateTime.UtcNow.AddMonths(-3);
            trip4.RecordedTimeStamp = startTime;
            trip4.EndTimeStamp = startTime;
            timeIncrement = 1;
            AddTripDetails(trip4, 2, 47.738929, -122.185166, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip4, 3, 47.738930, -122.185167, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip4, 4, 47.738928, -122.185167, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip4, 5, 47.738926, -122.185166, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip4, 6, 47.738923, -122.185166, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip4, 7, 47.738924, -122.185166, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip4, 8, 47.738924, -122.185168, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip4, 9, 47.738925, -122.185168, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip4, 10, 47.738926, -122.185167, startTime.AddMinutes(timeIncrement++));

            Trip trip5 = new Trip();
            trip5.UserId = "Amanda";
            trip5.Name = trip5.UserId + " - SF";
            trip5.Distance = 3;

            startTime = DateTime.Now.AddYears(-4);
            trip5.RecordedTimeStamp = startTime;
            trip5.EndTimeStamp = startTime;
            timeIncrement = 1;
            AddTripDetails(trip5, 2, 37.63973671, -122.44194609, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 3, 37.63997584, -122.44214793, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 4, 37.64021602, -122.44235831, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 5, 37.64045755, -122.44257364, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 6, 37.64069467, -122.44278177, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 7, 37.64093544, -122.44298813, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 8, 37.64117709, -122.44319877, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 9, 37.64141413, -122.44341636, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 10, 37.64164233, -122.44364393, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 11, 37.64186609, -122.44387452, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 12, 37.64209642, -122.44410611, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 13, 37.64230429, -122.44437793, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 14, 37.64250521, -122.44464263, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 15, 37.64270168, -122.44491739, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 16, 37.64289488, -122.44520296, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 17, 37.64307505, -122.44550295, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 18, 37.6432535, -122.44579707, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 19, 37.64341758, -122.44610972, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 20, 37.6435871, -122.4464253, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 21, 37.64375562, -122.44674171, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 22, 37.6439212, -122.44706014, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 23, 37.64408553, -122.44736776, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 24, 37.64425086, -122.44768593, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 25, 37.64442776, -122.44799045, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 26, 37.64460123, -122.44828122, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 27, 37.64480118, -122.44856712, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 28, 37.6450036, -122.44884951, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 29, 37.64522434, -122.4491044, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 30, 37.64544914, -122.44932703, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 31, 37.64568224, -122.44954965, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 32, 37.64593185, -122.44975467, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 33, 37.64618218, -122.44995475, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 34, 37.64643992, -122.45012096, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 35, 37.64671535, -122.45027846, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 36, 37.64697783, -122.45043973, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 37, 37.64724442, -122.45059714, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 38, 37.64750744, -122.45075656, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 39, 37.64777453, -122.45090651, startTime.AddMinutes(timeIncrement++));
            AddTripDetails(trip5, 40,  37.64803999, -122.45107063, startTime.AddMinutes(timeIncrement++));
			AddTripDetails(trip5, 41,  37.64830846, -122.45123173, startTime.AddMinutes(timeIncrement++));


            Trip trip6 = null;
            try
            {
                var json = ResourceLoader.GetEmbeddedResourceString(Assembly.Load(new AssemblyName("MyTrips.DataStore.Mock")), "sampletrip.json");
                trip6 = JsonConvert.DeserializeObject<Trip>(json);
                trip6.Photos = new List<Photo>();
                trip6.MainPhotoUrl = "http://www.livingwilderness.com/seattle/space-needle-fog.jpg";

                foreach (var pt in trip6.Points)
                {
                    pt.EngineLoad = random.Next(25, 75);
                    pt.EngineFuelRate = random.Next(19, 25);
                    pt.HasOBDData = true;
                }
            }
            catch (Exception ex)
            {
            }



            var items = new List<Trip>
            { 
                trip1, trip2, trip3, trip4, trip5, trip6
            };

            foreach (var item in items)
                item.Rating = random.Next(30, 100);

            return items;
        }

        public override Task InitializeStoreAsync()
        {
            if (initialized)
                return Task.FromResult(true);

            initialized = true;
            
            Trips = GetTrips();
            return Task.FromResult(true);
        }

        public override async Task<IEnumerable<Trip>> GetItemsAsync(int skip = 0, int take = 100, bool forceRefresh = false)
        {
            if (!initialized)
                await InitializeStoreAsync();
            if (photoStore == null)
                photoStore = Utils.ServiceLocator.Instance.Resolve<IPhotoStore>();

            foreach (var trip in Trips)
            {
                if (trip.Photos == null)
                    trip.Photos = new List<Photo>();
                trip.Photos.Clear();
                foreach (var photo in await photoStore.GetTripPhotos(trip.Id))
                    trip.Photos.Add(photo);
            }
            
            return Trips.OrderByDescending(s => s.RecordedTimeStamp);
        }

        public override async Task<Trip> GetItemAsync(string id)
        {
            if (!initialized)
                await InitializeStoreAsync();


            var trip = Trips.FirstOrDefault(t => t.Id == id);

            if (trip == null)
                trip = Trips[0];

            return trip;
        }

        public override async Task<bool> InsertAsync(Trip item)
        {
            //No need to set the Id here since it's already set in the BaseDataObject
            Trips.Add(item);
            return true;
        }

        public override Task<bool> RemoveAsync(Trip item)
        {
            Trips.Remove(item);
            return Task.FromResult(true);
        }
    }
}

