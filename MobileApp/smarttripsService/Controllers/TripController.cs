using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using MyTrips.DataObjects;
using smarttripsService.Models;

namespace smarttripsService.Controllers
{
    public class TripController : TableController<Trip>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            smarttripsContext context = new smarttripsContext();
            DomainManager = new EntityDomainManager<Trip>(context, Request);
        }

        // GET tables/TodoItem
        public IQueryable<Trip> GetAllTrips()
        {
            return Query();
        }

        // GET tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Trip> GetTrip(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Trip> PatchTrip(string id, Delta<Trip> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/TodoItem
        public async Task<IHttpActionResult> PostTrip(Trip trip)
        {
            Trip current = await InsertAsync(trip);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // PUT tables/Trip
        [HttpPut]
        public async Task<IHttpActionResult> EndTrip(string id)
        {
            var result = await LookupAsync(id);
            var trip = result.Queryable.First();
            trip.IsComplete = true;
            var replacedTrip = await DomainManager.ReplaceAsync(id, trip);
            return Ok(replacedTrip);
        }

        // DELETE tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteTrip(string id)
        {
            return DeleteAsync(id);
        }
    }
}
