// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using MyDriving.DataObjects;
using MyDrivingService.Helpers;
using MyDrivingService.Models;
using Microsoft.ApplicationInsights;

namespace MyDrivingService.Controllers
{
    public class TripController : TableController<Trip>
    {
        private MyDrivingContext dbContext;

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            dbContext = new MyDrivingContext();
            DomainManager = new EntityDomainManager<Trip>(dbContext, Request);
        }

        // GET tables/Trip
        [Authorize]
        public async Task<IQueryable<Trip>> GetAllTrips()
        {
            var id = await IdentitiyHelper.FindSidAsync(User, Request);
            if (string.IsNullOrWhiteSpace(id))
                return null;
            return Query().Where(s => s.UserId == id);
        }

        // GET tables/Trip/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [QueryableExpand("Points")]
        [Authorize]
        public SingleResult<Trip> GetTrip(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Trip/<id>
        [Authorize]
        public Task<Trip> PatchTrip(string id, Delta<Trip> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/Trip
        [Authorize]
        public async Task<IHttpActionResult> PostTrip(Trip trip)
        {
            var aiTelemetry = new TelemetryClient();
            var id = await IdentitiyHelper.FindSidAsync(User, Request);
            if (string.IsNullOrEmpty(id))
            {
                aiTelemetry.TrackEvent("UserId is null or empty!");
            }


            if (trip != null)
            {
                trip.UserId = id;
            }
            else
            {
                aiTelemetry.TrackEvent("Trip is null!");
                return BadRequest("Null trip");
            }


            Trip current = null;
            try
            {
                current = await InsertAsync(trip);
            }
            catch (HttpResponseException httpResponseException)
            {
                aiTelemetry.TrackException(httpResponseException);
                aiTelemetry.TrackEvent("Caught HttpResponseException. Response:" + httpResponseException.Response);
            }
            catch (System.Exception ex)
            {
                aiTelemetry.TrackException(ex);
            }

            if (current == null)
            {
                aiTelemetry.TrackEvent("Inserting trip failed!");
                return BadRequest("Inserting trip failed");
            }

            if (dbContext == null)
                dbContext = new MyDrivingContext();

            var curUser = dbContext.UserProfiles.FirstOrDefault(u => u.UserId == id);

            //update user with stats
            if (curUser != null)
            {
                curUser.FuelConsumption += trip.FuelUsed;

                var max = trip?.Points?.Max(s => s.Speed) ?? 0;
                if (max > curUser.MaxSpeed)
                    curUser.MaxSpeed = max;

                curUser.TotalDistance += trip.Distance;
                curUser.HardAccelerations += trip.HardAccelerations;
                curUser.HardStops += trip.HardStops;
                curUser.TotalTrips++;
                curUser.TotalTime += (long)(trip.EndTimeStamp - trip.RecordedTimeStamp).TotalSeconds;

                dbContext.SaveChanges();
            }
            else
            {
                aiTelemetry.TrackEvent("Cannot find user " + id);
            }

            //track large trips      
            var pointsCount = trip?.Points?.Count ?? 0;
            if (pointsCount > 1000)
                aiTelemetry.TrackEvent(string.Format("Saved large trip {0}. Points:{1}", current.Id, pointsCount));

            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Trip/<id>
        [Authorize]
        public Task DeleteTrip(string id)
        {
            return DeleteAsync(id);
        }
    }
}