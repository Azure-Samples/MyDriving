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
    public class UserController : TableController<UserProfile>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            smarttripsContext context = new smarttripsContext();
            DomainManager = new EntityDomainManager<UserProfile>(context, Request);
        }

        // GET tables/User
        [Authorize]
        public IQueryable<UserProfile> GetAllUsers()
        {
            return Query().Where(s => s.UserId == IdentitiyHelper.FindSid(this.User));
        }

        // GET tables/User/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [QueryableExpand("Devices")]
        [Authorize]
        public SingleResult<UserProfile> GetUser(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/User/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [Authorize]
        public Task<UserProfile> PatchUser(string id, Delta<UserProfile> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/User
        [Authorize]
        public async Task<IHttpActionResult> PostUser(UserProfile user)
        {
            var id = IdentitiyHelper.FindSid(this.User);
            user.UserId = id;
            UserProfile current = await InsertAsync(user);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/User/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [Authorize]
        public Task DeleteUser(string id)
        {
            return DeleteAsync(id);
        }
    }
}
