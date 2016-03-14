using Microsoft.Azure.Mobile.Server;
using MyTrips.DataObjects;
using smarttripsService.Helpers;
using smarttripsService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace smarttripsService.Controllers
{
    public class POIController : TableController<POI>
    {
        private smarttripsContext _dbContext;
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            _dbContext = new smarttripsContext();
            DomainManager = new EntityDomainManager<POI>(_dbContext, Request);
        }
        
        public IQueryable<POI> GetALlPOIs(string tripId)
        {
            return Query().Where(p => p.TripId == tripId);
        }        
    }
}
