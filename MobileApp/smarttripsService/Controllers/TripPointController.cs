using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using smarttripsService.DataObjects;
using smarttripsService.Models;

namespace smarttripsService.Controllers
{
    public class TripPointController : TableController<TripPoint>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            smarttripsContext context = new smarttripsContext();
            DomainManager = new EntityDomainManager<TripPoint>(context, Request);
        }

        // GET tables/TodoItem
        public IQueryable<TripPoint> GetAllTripPoints()
        {
            return Query();
        }

        // GET tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<TripPoint> GetTripPoint(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<TripPoint> PatchTripPoint(string id, Delta<TripPoint> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/TodoItem
        public async Task<IHttpActionResult> PostTripPoint(TripPoint tripPoint)
        {
            TripPoint current = await InsertAsync(tripPoint);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteTripPoint(string id)
        {
            return DeleteAsync(id);
        }
    }
}
