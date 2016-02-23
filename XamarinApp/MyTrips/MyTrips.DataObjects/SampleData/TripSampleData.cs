using MyTrips.DataObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrips.SampleData
{
    //This class is temporary - it will be used for getting trip data until the backend mobile app service is created.
    //Both the past trip and current trip data will eventually be retrieved from the mobile app service.
   public class TripSampleData
    {
        private static void AddTripDetails(Trip trip, int id, double lat, double lng, DateTime timestamp)
        {
            Trail pt = new Trail();
            pt.TrailId = id;
            pt.Latitude = lat;
            pt.Longitude = lng;
            pt.TimeStamp = timestamp;
            trip.Trail.Add(pt);

            Random r = new Random();
            Telemetry t1 = new Telemetry();
            t1.Key = "Speed";
            t1.Value = r.Next(30, 50).ToString();
            pt.Telemetry.Add(t1);
        }

        public static List<Trip> GetTrips()
        {
            Trip trip1 = new Trip();
            trip1.UserId = "Scott";
            trip1.TripId = trip1.UserId + "@Redmond";
            trip1.DaysSinceRecording = "1 day ago";
            trip1.TotalDistance = "34 miles";

            DateTime startTime = DateTime.Now;
            int timeIncrement = 1;
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

            Trip trip2 = new Trip();
            trip2.UserId = "Cindy";
            trip2.TripId = trip2.UserId + "@Seattle";
            trip2.DaysSinceRecording = "10 days ago";
            trip2.TotalDistance = "22 miles";

            startTime = DateTime.Now.AddDays(-2.0);
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
            trip3.TripId = trip3.UserId + "@Portland";
            trip3.DaysSinceRecording = "1 month ago";
            trip3.TotalDistance = "173 miles";

            startTime = DateTime.Now.AddDays(-3.0);
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
            trip4.TripId = trip3.UserId + "@Bellevue";
            trip4.DaysSinceRecording = "3 months ago";
            trip4.TotalDistance = "22 miles";

            startTime = DateTime.Now.AddDays(-3.0);
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

            List<Trip> trips = new List<Trip>();
            trips.Add(trip1);
            trips.Add(trip2);
            trips.Add(trip3);
            trips.Add(trip4);

            return trips;
        }
    }
}
