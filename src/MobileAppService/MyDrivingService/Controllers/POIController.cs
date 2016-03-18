using Microsoft.Azure.Mobile.Server;
using MyDriving.DataObjects;
using MyDrivingService.Helpers;
using MyDrivingService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace MyDrivingService.Controllers
{
    public class POIController : TableController<POI>
    {
        private MyDrivingContext _dbContext;
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            _dbContext = new MyDrivingContext();
            DomainManager = new EntityDomainManager<POI>(_dbContext, Request);
        }
        
        public IQueryable<POI> GetAllPOIs(string tripId)
        {
            return Query().Where(p => p.TripId == tripId);
        }        
    }
}
