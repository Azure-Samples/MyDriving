using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using MyDriving.DataObjects;
using MyDrivingService.Models;
using System;
using System.Collections.Generic;

namespace MyDrivingService.Controllers
{
    public class TripPointController : TableController<TripPoint>
    {
        private MyDrivingContext _dbContext;

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MyDrivingContext context = new MyDrivingContext();
            this._dbContext = context;
            DomainManager = new EntityDomainManager<TripPoint>(context, Request);
        }

        // GET tables/TripPoint
       //[Authorize]
        public IQueryable<TripPoint> GetAllTripPoints()
        {
            return Query();
        }

        // GET tables/TripPoint/<id>
       //[Authorize]
        public SingleResult<TripPoint> GetTripPoint(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/TripPoint/<id>
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

        // DELETE tables/TripPoint/<id>
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
