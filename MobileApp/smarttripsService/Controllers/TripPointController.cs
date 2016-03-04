using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using MyTrips.DataObjects;
using smarttripsService.Models;
using System;
using System.Collections.Generic;

namespace smarttripsService.Controllers
{
    public class TripPointController : TableController<TripPoint>
    {
        private smarttripsContext _dbContext;

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            smarttripsContext context = new smarttripsContext();
            this._dbContext = context;
            DomainManager = new EntityDomainManager<TripPoint>(context, Request);
        }

        // GET tables/TodoItem
       //[Authorize]
        public IQueryable<TripPoint> GetAllTripPoints()
        {
            return Query();
        }

        // GET tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
       //[Authorize]
        public SingleResult<TripPoint> GetTripPoint(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
       //[Authorize]
        public Task<TripPoint> PatchTripPoint(string id, Delta<TripPoint> patch)
        {
            return UpdateAsync(id, patch);
        }


        /*public async Task<IHttpActionResult> PostTripPoints(IEnumerable<TripPoint> tripPoints)
        {
            foreach(var point in tripPoints)
            {
                var pointTrip = this._dbContext.Trips.Where(trip => trip.Id == point.TripId).FirstOrDefault();
                if(pointTrip == null)
                {
                    return BadRequest("The trip you are trying to add points to does not exist.");
                } else if(pointTrip.IsComplete)
                {
                    return BadRequest("The trip you are trying to add points to is already complete.");
                }
                else
                {
                    var maxSequence = getMaxSequence(point.TripId);
                    maxSequence++;
                    point.Sequence = maxSequence;

                    TripPoint current = await InsertAsync(point);
                }
            }
            return Created("tables/trippoints", tripPoints);
        }*/

        // DELETE tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
       //[Authorize]
        public Task DeleteTripPoint(string id)
        {
            return DeleteAsync(id);
        }

        private int getMaxSequence(string tripId)
        {
            var tripPoints = Query();
            return tripPoints.Where(point => point.TripId == tripId).Max(point => point.Sequence);
        }
    }
}
