using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using MobileApp.DataObjects;
using MobileApp.Models;

namespace MobileApp.Controllers
{
    public class TripItemController : TableController<TripItem>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<TripItem>(context, Request);
        }

        // GET tables/TodoItem
        public IQueryable<TripItem> GetAllTripItems()
        {
            return Query();
        }

        // GET tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<TripItem> GetTripItem(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<TripItem> PatchTripItem(string id, Delta<TripItem> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/TodoItem
        public async Task<IHttpActionResult> PostTripItem(TripItem item)
        {
            TripItem current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteTripItem(string id)
        {
            return DeleteAsync(id);
        }
    }
}