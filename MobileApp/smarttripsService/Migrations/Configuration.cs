namespace smarttripsService.Migrations
{
    using DataObjects;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<smarttripsService.Models.smarttripsContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(smarttripsService.Models.smarttripsContext context)
        {
            seedTrips(context);
            seedTripPoints(context);
        }
        private void seedTripPoints(smarttripsService.Models.smarttripsContext context)
        {
            foreach (var point in context.TripPoints)
            {
                context.TripPoints.Remove(point);
            }
            List<TripPoint> tripPoints = new List<TripPoint>
            {
                new TripPoint {Id="1", TripId = "1", Sequence = 1, Latitude = 47.606557f, Longitude = -122.336555f, DateTime = new DateTime(2016,2,1, 12,34,50)},
                new TripPoint {Id="2", TripId = "1", Sequence = 2, Latitude = 47.606343f, Longitude = -122.337002f, DateTime = new DateTime(2016,2,1, 12,34,55)},
                new TripPoint {Id="3", TripId = "1", Sequence = 3, Latitude = 47.606142f, Longitude = -122.337488f, DateTime = new DateTime(2016,2,1, 12,35,0)},
                new TripPoint {Id="4", TripId = "1", Sequence = 4, Latitude = 47.606519f, Longitude = -122.337898f, DateTime = new DateTime(2016,2,1, 12,35,5)},
                new TripPoint {Id="5", TripId = "1", Sequence = 5, Latitude = 47.606847f, Longitude = -122.338197f, DateTime = new DateTime(2016,2,1, 12,35,10)},
                new TripPoint {Id="6", TripId = "1", Sequence = 6, Latitude = 47.607111f, Longitude = -122.337618f, DateTime = new DateTime(2016,2,1, 12,35,15)},
                new TripPoint {Id="7", TripId = "1", Sequence = 7, Latitude = 47.607425f, Longitude = -122.336946f, DateTime = new DateTime(2016,2,1, 12,35,20)},
                new TripPoint {Id="8", TripId = "1", Sequence = 8, Latitude = 47.607689f, Longitude = -122.326256f, DateTime = new DateTime(2016,2,1, 12,35,25)},
                new TripPoint {Id="9", TripId = "1", Sequence = 9, Latitude = 47.607941f, Longitude = -122.335622f, DateTime = new DateTime(2016,2,1, 12,35,30)},
                new TripPoint {Id="10", TripId = "1", Sequence = 10, Latitude = 47.608230f, Longitude = -122.334875f, DateTime = new DateTime(2016,2,1, 12,35,35)},
                new TripPoint {Id="11", TripId = "1", Sequence = 11, Latitude = 47.608444f, Longitude = -122.334353f, DateTime = new DateTime(2016,2,1, 12,35,40)},
                new TripPoint {Id="12", TripId = "1", Sequence = 12, Latitude = 47.608670f, Longitude = -122.333719f, DateTime = new DateTime(2016,2,1, 12,35,45)},
                new TripPoint {Id="13", TripId = "1", Sequence = 13, Latitude = 47.608394f, Longitude = -122.333551f, DateTime = new DateTime(2016,2,1, 12,35,50)},
                new TripPoint {Id="14", TripId = "1", Sequence = 14, Latitude = 47.608092f, Longitude = -122.333289f, DateTime = new DateTime(2016,2,1, 12,35,55)},
                new TripPoint {Id="15", TripId = "1", Sequence = 15, Latitude = 47.607840f, Longitude = -122.333028f, DateTime = new DateTime(2016,2,1, 12,36,0)},
                new TripPoint {Id="16", TripId = "1", Sequence = 16, Latitude = 47.607538f, Longitude = -122.332711f, DateTime = new DateTime(2016,2,1, 12,36,5)},

                new TripPoint {Id="17", TripId = "2", Sequence = 1, Latitude = 37.807055f, Longitude = -122.475603f, DateTime = new DateTime(2015,1,18, 20,14,20)},
                new TripPoint {Id="18", TripId = "2", Sequence = 2, Latitude = 37.807689f, Longitude = -122.476182f, DateTime = new DateTime(2015,1,18, 20,14,25)},
                new TripPoint {Id="19", TripId = "2", Sequence = 3, Latitude = 37.808234f, Longitude = -122.476685f, DateTime = new DateTime(2015,1,18, 20,14,30)},
                new TripPoint {Id="20", TripId = "2", Sequence = 4, Latitude = 37.808765f, Longitude = -122.477152f, DateTime = new DateTime(2015,1,18, 20,14,35)},
                new TripPoint {Id="21", TripId = "2", Sequence = 5, Latitude = 37.809414f, Longitude = -122.477413f, DateTime = new DateTime(2015,1,18, 20,14,40)},
                new TripPoint {Id="22", TripId = "2", Sequence = 6, Latitude = 37.810062f, Longitude = -122.477506f, DateTime = new DateTime(2015,1,18, 20,14,45)},
                new TripPoint {Id="23", TripId = "2", Sequence = 7, Latitude = 37.810652f, Longitude = -122.477544f, DateTime = new DateTime(2015,1,18, 20,14,50)},
                new TripPoint {Id="24", TripId = "2", Sequence = 8, Latitude = 37.811301f, Longitude = -122.477600f, DateTime = new DateTime(2015,1,18, 20,14,55)},
                new TripPoint {Id="25", TripId = "2", Sequence = 9, Latitude = 37.811934f, Longitude = -122.477693f, DateTime = new DateTime(2015,1,18, 20,15,0)},
                new TripPoint {Id="26", TripId = "2", Sequence = 10, Latitude = 37.812711f, Longitude = -122.477819f, DateTime = new DateTime(2015,1,18, 20,15,5)},
                new TripPoint {Id="27", TripId = "2", Sequence = 11, Latitude = 37.813316f, Longitude = -122.477893f, DateTime = new DateTime(2015,1,18, 20,15,10)},
                new TripPoint {Id="28", TripId = "2", Sequence = 12, Latitude = 37.814008f, Longitude = -122.477949f, DateTime = new DateTime(2015,1,18, 20,15,15)},
                new TripPoint {Id="29", TripId = "2", Sequence = 13, Latitude = 37.814787f, Longitude = -122.478010f, DateTime = new DateTime(2015,1,18, 20,15,20)},
                new TripPoint {Id="30", TripId = "2", Sequence = 14, Latitude = 37.815495f, Longitude = -122.478085f, DateTime = new DateTime(2015,1,18, 20,15,25)}
            };

            foreach (TripPoint point in tripPoints)
            {
                context.Set<TripPoint>().Add(point);
            }
        }
        private void seedTrips(smarttripsService.Models.smarttripsContext context)
        {
            foreach (var trip in context.Trips)
            {
                context.Trips.Remove(trip);
            }

            List<Trip> trips = new List<Trip>
            {
                new Trip {
                    Id = "1",
                    Name = "Haishi@Seattle",
                    IsComplete = true,
                    StartDate = new DateTime(2016,2,1, 12,34,50),
                    StartingLatitude = 47.606557f,
                    StartingLongitude = -122.336555f,
                    Rate = 5,
                    UserId = "hbai@microsoft.com"
                    },
                new Trip {
                    Id = "2",
                    Name = "Scott@San Francisco",
                    IsComplete = true,
                    StartDate = new DateTime(2015,1,18, 20,14,20),
                    StartingLatitude = 37.807055f,
                    StartingLongitude = -122.475603f,
                    Rate = 3,
                    UserId = "scott.gu2016@outlook.com"
                    },
            };

            foreach (Trip trip in trips)
            {
                context.Set<Trip>().Add(trip);
            }
        }
    }
}
