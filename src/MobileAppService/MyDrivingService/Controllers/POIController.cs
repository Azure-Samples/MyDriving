// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Linq;
using System.Web.Http.Controllers;
using Microsoft.Azure.Mobile.Server;
using MyDriving.DataObjects;
using MyDrivingService.Models;
using System.Web.Http;

namespace MyDrivingService.Controllers
{
    public class POIController : TableController<POI>
    {
        private MyDrivingContext dbContext;

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            dbContext = new MyDrivingContext();
            DomainManager = new EntityDomainManager<POI>(dbContext, Request);
        }
       
        [Authorize]
        public IQueryable<POI> GetAllPOIs(string tripId)
        {
            return Query().Where(p => p.TripId == tripId);
        }
    }
}