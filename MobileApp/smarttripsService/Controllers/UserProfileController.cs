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
    public class UserProfileController : TableController<UserProfile>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            smarttripsContext context = new smarttripsContext();
            DomainManager = new EntityDomainManager<UserProfile>(context, Request);
        }

        // GET tables/UserProfile
        //[Authorize]
        [QueryableExpand("Devices")]
        public IQueryable<UserProfile> GetAllUsers()
        {
            //TODO: remove and add authorize in future
            var id = IdentitiyHelper.FindSid(this.User);
            if (string.IsNullOrWhiteSpace(id))
                return Query();
            return Query().Where(s => s.UserId == id);
        }

        // GET tables/UserProfile/<id>
        //[Authorize]
        [QueryableExpand("Devices")]
        public SingleResult<UserProfile> GetUser(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/UserProfile/<id>
        //[Authorize]
        public Task<UserProfile> PatchUser(string id, Delta<UserProfile> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/UserProfile
       //[Authorize]
        public async Task<IHttpActionResult> PostUser(UserProfile user)
        {
            var id = IdentitiyHelper.FindSid(this.User);
            user.UserId = id;
            UserProfile current = await InsertAsync(user);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/UserProfile/<id>
       //[Authorize]
        public Task DeleteUser(string id)
        {
            return DeleteAsync(id);
        }
    }
}
