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
        //[Authorize]
        //[QueryableExpand("Points")]
        public async Task<IQueryable<Trip>> GetAllTrips()
        {
            var id = await IdentitiyHelper.FindSidAsync(User, Request);
            if (string.IsNullOrWhiteSpace(id))
                return Query();
            return Query().Where(s => s.UserId == id);
        }

        // GET tables/Trip/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [QueryableExpand("Points")]
        //[Authorize]
        public SingleResult<Trip> GetTrip(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Trip/<id>
        //[Authorize]
        public Task<Trip> PatchTrip(string id, Delta<Trip> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/Trip
        //[Authorize]
        public async Task<IHttpActionResult> PostTrip(Trip trip)
        {
            var id = await IdentitiyHelper.FindSidAsync(User, Request);
            trip.UserId = id;


            Trip current = await InsertAsync(trip);

            if (dbContext == null)
                dbContext = new MyDrivingContext();

            var curUser = dbContext.UserProfiles.FirstOrDefault(u => u.UserId == id);

            //update user with stats
            if (curUser != null)
            {
                curUser.FuelConsumption += current.FuelUsed;

                var max = current?.Points.Max(s => s.Speed) ?? 0;
                if (max > curUser.MaxSpeed)
                    curUser.MaxSpeed = max;

                curUser.TotalDistance += current.Distance;
                curUser.HardAccelerations += current.HardAccelerations;
                curUser.HardStops += current.HardStops;
                curUser.TotalTrips++;
                curUser.TotalTime += (long) (current.EndTimeStamp - current.RecordedTimeStamp).TotalSeconds;

                dbContext.SaveChanges();
            }


            return CreatedAtRoute("Tables", new {id = current.Id}, current);
        }

        // DELETE tables/Trip/<id>
        //[Authorize]
        public Task DeleteTrip(string id)
        {
            return DeleteAsync(id);
        }
    }
}