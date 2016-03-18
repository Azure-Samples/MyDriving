// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Linq;
using System.Web.Http.Controllers;
using Microsoft.Azure.Mobile.Server;
using MyDriving.DataObjects;
using MyDrivingService.Models;

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