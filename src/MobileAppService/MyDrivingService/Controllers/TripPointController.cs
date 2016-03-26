// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using MyDriving.DataObjects;
using MyDrivingService.Models;
using MyDrivingService.Helpers;

namespace MyDrivingService.Controllers
{
    public class TripPointController : TableController<TripPoint>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MyDrivingContext context = new MyDrivingContext();
            DomainManager = new EntityDomainManager<TripPoint>(context, Request);
        }

        // GET tables/TripPoint
        [EnableQuery(MaxTop = 100000)]
        [Authorize]
        public IQueryable<TripPoint> GetAllTripPoints()
        {
            return Query();
        }

        // GET tables/TripPoint/<id>
        [Authorize]
        public SingleResult<TripPoint> GetTripPoint(string id)
        {
            return Lookup(id);
        }

        // POST tables/TripPoint
        [Authorize]
        public async Task<IHttpActionResult> PostTripPoint(TripPoint tripPoint)
        {
 
            var current = await InsertAsync(tripPoint);

            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // PATCH tables/TripPoint/<id>
        [Authorize]
        public Task<TripPoint> PatchTripPoint(string id, Delta<TripPoint> patch)
        {
            return UpdateAsync(id, patch);
        }

        // DELETE tables/TripPoint/<id>
        [Authorize]
        public Task DeleteTripPoint(string id)
        {
            return DeleteAsync(id);
        }

        private int GetMaxSequence(string tripId)
        {
            var tripPoints = Query();
            return tripPoints.Where(point => point.TripId == tripId).Max(point => point.Sequence);
        }
    }
}