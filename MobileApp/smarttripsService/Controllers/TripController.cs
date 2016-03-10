using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using MyTrips.DataObjects;
using smarttripsService.Models;
using smarttripsService.Helpers;

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

        // GET tables/Trip
        //[Authorize]
        [QueryableExpand("Points,Tips")]
        public IQueryable<Trip> GetAllTrips()
        {
            var id = IdentitiyHelper.FindSid(this.User);
            if (string.IsNullOrWhiteSpace(id))
                return Query();
            return Query().Where(s => s.UserId == id);
        }

        // GET tables/Trip/<id>
        [QueryableExpand("Points,Tips")]
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
            var id = IdentitiyHelper.FindSid(this.User);
            trip.UserId = id;
            Trip current = await InsertAsync(trip);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Trip/<id>
       //[Authorize]
        public Task DeleteTrip(string id)
        {
            return DeleteAsync(id);
        }
    }
}
