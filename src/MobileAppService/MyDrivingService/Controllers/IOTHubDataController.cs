// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using MyDriving.DataObjects;
using MyDrivingService.Models;

namespace MyDrivingService.Controllers
{
    public class IOTHubDataController : TableController<IOTHubData>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MyDrivingContext context = new MyDrivingContext();
            DomainManager = new EntityDomainManager<IOTHubData>(context, Request);
        }

        // GET tables/IOTHubData
        [Authorize]
        public IQueryable<IOTHubData> GetAllIOTHubData()
        {
            return Query();
        }

        // GET tables/IOTHubData/<id>
        [Authorize]
        public SingleResult<IOTHubData> GetIOTHubData(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/IOTHubData/<id>
        [Authorize]
        public Task<IOTHubData> PatchIOTHubData(string id, Delta<IOTHubData> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/IOTHubData
        [Authorize]
        public async Task<IHttpActionResult> PostIOTHubData(IOTHubData item)
        {
            IOTHubData current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new {id = current.Id}, current);
        }

        // DELETE tables/IOTHubData/<id>
        [Authorize]
        public Task DeleteIOTHubData(string id)
        {
            return DeleteAsync(id);
        }
    }
}