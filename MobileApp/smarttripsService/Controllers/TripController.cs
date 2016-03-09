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
        private smarttripsContext _dbContext;
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            _dbContext = new smarttripsContext();
            DomainManager = new EntityDomainManager<Trip>(_dbContext, Request);
        }

        // GET tables/trip
        //[Authorize]
        [QueryableExpand("Points")]
        public IQueryable<Trip> GetAllTrips()
        {
            var id = IdentitiyHelper.FindSid(this.User);
            if (string.IsNullOrWhiteSpace(id))
                return Query();
            return Query().Where(s => s.UserId == id);
        }

        // GET tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [QueryableExpand("Points")]
       //[Authorize]
        public SingleResult<Trip> GetTrip(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
       //[Authorize]
        public Task<Trip> PatchTrip(string id, Delta<Trip> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/TodoItem
       //[Authorize]
        public async Task<IHttpActionResult> PostTrip(Trip trip)
        {
            var id = IdentitiyHelper.FindSid(this.User);
            trip.UserId = id;


            Trip current = await InsertAsync(trip);

            if (_dbContext == null)
                _dbContext = new smarttripsContext();

            var curUser = _dbContext.UserProfiles.FirstOrDefault(u => u.UserId == id);

            //update user with stats
            if (curUser != null)
            {
                curUser.FuelConsumption += current.FuelUsed;

                var max = current?.Points.Max(s => s.Speed) ?? 0;
                if (max > curUser.MaxSpeed)
                    curUser.MaxSpeed = max;

                curUser.TotalDistance += current.Distance;
                curUser.HardAcceleration += current.HardAcceleration;
                curUser.HardStops += current.HardStops;
                curUser.TotalTrips++;
                curUser.TotalTime += (long)(current.RecordedTimeStamp - current.EndTimeStamp).TotalSeconds;

                _dbContext.SaveChanges();
            }


            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
       //[Authorize]
        public Task DeleteTrip(string id)
        {
            return DeleteAsync(id);
        }
    }
}
