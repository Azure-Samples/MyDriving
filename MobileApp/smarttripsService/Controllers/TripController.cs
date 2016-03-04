using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using MyTrips.DataObjects;
using smarttripsService.Models;
using smarttripsService.Helpers;
using System.Web;

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
        [Authorize]
        public IQueryable<Trip> GetAllTrips()
        {
            return Query().Where(s => s.UserId == IdentitiyHelper.FindSid(this.User));
        }

        // GET tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [QueryableExpand("Points,Tips")]
        [Authorize]
        public SingleResult<Trip> GetTrip(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [Authorize]
        public Task<Trip> PatchTrip(string id, Delta<Trip> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/TodoItem
        [Authorize]
        public async Task<IHttpActionResult> PostTrip(Trip trip)
        {
            var id = IdentitiyHelper.FindSid(this.User);
            trip.UserId = id;
            Trip current = await InsertAsync(trip);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [Authorize]
        public Task DeleteTrip(string id)
        {
            return DeleteAsync(id);
        }
    }
}
