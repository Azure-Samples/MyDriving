using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.Azure.Mobile.Server.Config;
using MyTrips.DataObjects;
using smarttripsService.Models;
using Owin;

namespace smarttripsService
{
    public partial class Startup
    {
        public static void ConfigureMobileApp(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            //For more information on Web API tracing, see http://go.microsoft.com/fwlink/?LinkId=620686 
            config.EnableSystemDiagnosticsTracing();

            new MobileAppConfiguration()
                .UseDefaultConfiguration()
                .ApplyTo(config);

            // Use Entity Framework Code First to create database tables based on your DbContext
            Database.SetInitializer(new smarttripsInitializer());

            // To prevent Entity Framework from modifying your database schema, use a null database initializer
            // Database.SetInitializer<smarttripsContext>(null);

            MobileAppSettingsDictionary settings = config.GetMobileAppSettingsProvider().GetMobileAppSettings();

            if (string.IsNullOrEmpty(settings.HostName))
            {
                // This middleware is intended to be used locally for debugging. By default, HostName will
                // only have a value when running in an App Service application.
                app.UseAppServiceAuthentication(new AppServiceAuthenticationOptions
                {
                    SigningKey = ConfigurationManager.AppSettings["SigningKey"],
                    ValidAudiences = new[] { ConfigurationManager.AppSettings["ValidAudience"] },
                    ValidIssuers = new[] { ConfigurationManager.AppSettings["ValidIssuer"] },
                    TokenHandler = config.GetAppServiceTokenHandler()
                });
            }
            app.UseWebApi(config);
        }
    }

    public class smarttripsInitializer : CreateDatabaseIfNotExists<smarttripsContext>
    {
        protected override void Seed(smarttripsContext context)
        {
            seedTrips(context);
            seedTripPoints(context);
            base.Seed(context);
        }
        private void seedTripPoints(smarttripsService.Models.smarttripsContext context)
        {
            foreach (var point in context.TripPoints)
            {
                context.TripPoints.Remove(point);
            }
            List<TripPoint> tripPoints = new List<TripPoint>
            {
                new TripPoint {TripId = "1", Sequence = 1, Latitude = 47.606557f, Longitude = -122.336555f, TimeStamp = new DateTime(2016,2,1, 12,34,50)},
                new TripPoint {TripId = "1", Sequence = 2, Latitude = 47.606343f, Longitude = -122.337002f, TimeStamp = new DateTime(2016,2,1, 12,34,55)},
                new TripPoint {TripId = "1", Sequence = 3, Latitude = 47.606142f, Longitude = -122.337488f, TimeStamp = new DateTime(2016,2,1, 12,35,0)},
                new TripPoint {TripId = "1", Sequence = 4, Latitude = 47.606519f, Longitude = -122.337898f, TimeStamp = new DateTime(2016,2,1, 12,35,5)},
                new TripPoint {TripId = "1", Sequence = 5, Latitude = 47.606847f, Longitude = -122.338197f, TimeStamp = new DateTime(2016,2,1, 12,35,10)},
                new TripPoint {TripId = "1", Sequence = 6, Latitude = 47.607111f, Longitude = -122.337618f, TimeStamp = new DateTime(2016,2,1, 12,35,15)},
                new TripPoint {TripId = "1", Sequence = 7, Latitude = 47.607425f, Longitude = -122.336946f, TimeStamp = new DateTime(2016,2,1, 12,35,20)},
                new TripPoint {TripId = "1", Sequence = 8, Latitude = 47.607689f, Longitude = -122.326256f, TimeStamp = new DateTime(2016,2,1, 12,35,25)},
                new TripPoint {TripId = "1", Sequence = 9, Latitude = 47.607941f, Longitude = -122.335622f, TimeStamp = new DateTime(2016,2,1, 12,35,30)},
                new TripPoint {TripId = "1", Sequence = 10, Latitude = 47.608230f, Longitude = -122.334875f, TimeStamp = new DateTime(2016,2,1, 12,35,35)},
                new TripPoint {TripId = "1", Sequence = 11, Latitude = 47.608444f, Longitude = -122.334353f, TimeStamp = new DateTime(2016,2,1, 12,35,40)},
                new TripPoint {TripId = "1", Sequence = 12, Latitude = 47.608670f, Longitude = -122.333719f, TimeStamp = new DateTime(2016,2,1, 12,35,45)},
                new TripPoint {TripId = "1", Sequence = 13, Latitude = 47.608394f, Longitude = -122.333551f, TimeStamp = new DateTime(2016,2,1, 12,35,50)},
                new TripPoint {TripId = "1", Sequence = 14, Latitude = 47.608092f, Longitude = -122.333289f, TimeStamp = new DateTime(2016,2,1, 12,35,55)},
                new TripPoint {TripId = "1", Sequence = 15, Latitude = 47.607840f, Longitude = -122.333028f, TimeStamp = new DateTime(2016,2,1, 12,36,0)},
                new TripPoint {TripId = "1", Sequence = 16, Latitude = 47.607538f, Longitude = -122.332711f, TimeStamp = new DateTime(2016,2,1, 12,36,5)},

                new TripPoint {TripId = "2", Sequence = 1, Latitude = 37.807055f, Longitude = -122.475603f, TimeStamp = new DateTime(2015,1,18, 20,14,20)},
                new TripPoint {TripId = "2", Sequence = 2, Latitude = 37.807689f, Longitude = -122.476182f, TimeStamp = new DateTime(2015,1,18, 20,14,25)},
                new TripPoint {TripId = "2", Sequence = 3, Latitude = 37.808234f, Longitude = -122.476685f, TimeStamp = new DateTime(2015,1,18, 20,14,30)},
                new TripPoint {TripId = "2", Sequence = 4, Latitude = 37.808765f, Longitude = -122.477152f, TimeStamp = new DateTime(2015,1,18, 20,14,35)},
                new TripPoint {TripId = "2", Sequence = 5, Latitude = 37.809414f, Longitude = -122.477413f, TimeStamp = new DateTime(2015,1,18, 20,14,40)},
                new TripPoint {TripId = "2", Sequence = 6, Latitude = 37.810062f, Longitude = -122.477506f, TimeStamp = new DateTime(2015,1,18, 20,14,45)},
                new TripPoint {TripId = "2", Sequence = 7, Latitude = 37.810652f, Longitude = -122.477544f, TimeStamp = new DateTime(2015,1,18, 20,14,50)},
                new TripPoint {TripId = "2", Sequence = 8, Latitude = 37.811301f, Longitude = -122.477600f, TimeStamp = new DateTime(2015,1,18, 20,14,55)},
                new TripPoint {TripId = "2", Sequence = 9, Latitude = 37.811934f, Longitude = -122.477693f, TimeStamp = new DateTime(2015,1,18, 20,15,0)},
                new TripPoint {TripId = "2", Sequence = 10, Latitude = 37.812711f, Longitude = -122.477819f, TimeStamp = new DateTime(2015,1,18, 20,15,5)},
                new TripPoint {TripId = "2", Sequence = 11, Latitude = 37.813316f, Longitude = -122.477893f, TimeStamp = new DateTime(2015,1,18, 20,15,10)},
                new TripPoint {TripId = "2", Sequence = 12, Latitude = 37.814008f, Longitude = -122.477949f, TimeStamp = new DateTime(2015,1,18, 20,15,15)},
                new TripPoint {TripId = "2", Sequence = 13, Latitude = 37.814787f, Longitude = -122.478010f, TimeStamp = new DateTime(2015,1,18, 20,15,20)},
                new TripPoint {TripId = "2", Sequence = 14, Latitude = 37.815495f, Longitude = -122.478085f, TimeStamp = new DateTime(2015,1,18, 20,15,25)}
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
                    TimeStamp = new DateTime(2016,2,1, 12,34,50),

                    Rating = 90
                    },
                new Trip {
                    Id = "2",
                    Name = "Scott@San Francisco",
                    IsComplete = true,
                    TimeStamp = new DateTime(2015,1,18, 20,14,20),
                    Rating = 86
                    },
            };

            foreach (Trip trip in trips)
            {
                context.Set<Trip>().Add(trip);
            }
        }
    }
}

