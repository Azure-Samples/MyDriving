using System.Web.Http;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using System.Threading.Tasks;
using System.Collections.Generic;
using smarttripsService.Models;
using MyTrips.DataObjects;
using System.Linq;
using System;

namespace smarttripsService.Controllers
{
    // Use the MobileAppController attribute for each ApiController you want to use  
    // from your mobile clients 
    [MobileAppController]
    public class ProvisionController : ApiController
    {
        private static RegistryManager registryManager;
        private static object syncRoot = new object();

        // GET api/values
        [HttpGet]
       //[Authorize]
        public async Task<IEnumerable<Device>> Get()
        {
            EnsureRegistryManagerInitialized();
            return await GetDevices();
        }

        // POST api/values
        [HttpPost]
       //[Authorize]
        public async Task<IHttpActionResult> Post(string userId, string deviceName)
        {
            Device device;
            EnsureRegistryManagerInitialized();
            MobileAppSettingsDictionary settings = this.Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();

            var existingDevices = await GetDevices();
            int maxDevices = int.Parse(settings["MaxDevices"]);
            smarttripsContext context = new smarttripsContext();
            var curUser = context.UserProfiles.Where(user => user.UserId == userId).FirstOrDefault();
            if(curUser == null)
            {
                curUser = context.UserProfiles.Add(new MyTrips.DataObjects.UserProfile { Id = Guid.NewGuid().ToString(), UserId = userId });
                //return BadRequest("User has not authenticated with mobile app yet.");
            }
            if(curUser.Devices == null)
            {
                curUser.Devices = new List<string>();
            }
            if(curUser.Devices.Count >= maxDevices)
            {
                return BadRequest("You already have more than the maximum number of devices");
            }
            

            try
            {
                device = await registryManager.AddDeviceAsync(new Device(deviceName));
                curUser.Devices.Add(deviceName);
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
                lock (syncRoot)
                {
                    if (registryManager == null)
                    {
                        var settings = this.Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();
                        string iotHubconnectionString = settings["IoTHubConnectionString"]; //ConfigurationManager.AppSettings["IoTHubConnectionString"];
                        registryManager = RegistryManager.CreateFromConnectionString(iotHubconnectionString);
                    }
                }
            }
        }

        private async Task<IEnumerable<Device>> GetDevices()
        {
            MobileAppSettingsDictionary settings = this.Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();
            int maxDevices = int.Parse(settings["MaxDevices"]);

            return await registryManager.GetDevicesAsync(maxDevices);
        }
    }
}
