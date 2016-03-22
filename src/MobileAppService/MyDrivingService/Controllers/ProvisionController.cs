// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Config;
using MyDrivingService.Models;

namespace MyDrivingService.Controllers
{
    // Use the MobileAppController attribute for each ApiController you want to use from your mobile clients 
    [MobileAppController]
    public class ProvisionController : ApiController
    {
        private static RegistryManager registryManager;
        private static readonly object SyncRoot = new object();

        // GET api/Provision
        [HttpGet]
        //[Authorize]
        public async Task<IEnumerable<Device>> Get()
        {
            EnsureRegistryManagerInitialized();
            return await GetDevices();
        }

        // POST api/Provision
        [HttpPost]
        //[Authorize]
        public async Task<IHttpActionResult> Post(string userId, string deviceName)
        {
            Device device;
            EnsureRegistryManagerInitialized();
            MobileAppSettingsDictionary settings = Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();

            await GetDevices();
            int maxDevices = int.Parse(settings["MaxDevices"]);
            MyDrivingContext context = new MyDrivingContext();
            var curUser = context.UserProfiles.FirstOrDefault(user => user.UserId == userId) ??
                          context.UserProfiles.Add(new MyDriving.DataObjects.UserProfile
                          {
                              Id = Guid.NewGuid().ToString(),
                              UserId = userId
                          });

            if (curUser.Devices == null)
            {
                curUser.Devices = new List<MyDriving.DataObjects.Device>();
            }

            if (curUser.Devices.Count >= maxDevices)
            {
                return BadRequest("You already have more than the maximum number of devices");
            }

            try
            {
                device = await registryManager.AddDeviceAsync(new Device(deviceName));
                curUser.Devices.Add(new MyDriving.DataObjects.Device {Name = deviceName});
                await context.SaveChangesAsync();
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await registryManager.GetDeviceAsync(deviceName);
            }
            return Created("api/provision", device.Authentication.SymmetricKey.PrimaryKey);
        }

        private void EnsureRegistryManagerInitialized()
        {
            if (registryManager == null)
            {
                lock (SyncRoot)
                {
                    if (registryManager == null)
                    {
                        var settings = Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();
                        string iotHubconnectionString = settings["IoTHubConnectionString"];
                        //ConfigurationManager.AppSettings["IoTHubConnectionString"];
                        registryManager = RegistryManager.CreateFromConnectionString(iotHubconnectionString);
                    }
                }
            }
        }

        private async Task<IEnumerable<Device>> GetDevices()
        {
            MobileAppSettingsDictionary settings = Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();
            int maxDevices = int.Parse(settings["MaxDevices"]);

            return await registryManager.GetDevicesAsync(maxDevices);
        }
    }
}