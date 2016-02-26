using System.Web.Http;
using System.Web.Http.Tracing;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.Devices;
using System.Configuration;
using Microsoft.Azure.Devices.Common.Exceptions;
using System.Threading.Tasks;

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
        public string Get()
        {
            MobileAppSettingsDictionary settings = this.Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();
            
            ITraceWriter traceWriter = this.Configuration.Services.GetTraceWriter();

            string host = settings.HostName ?? "localhost";
            string greeting = "Hello from " + host;
            
            traceWriter.Info(greeting);
            return greeting;
        }

        // POST api/values
        public async Task<string> Post(string userId, string deviceName)
        {
            Device device;

            if (registryManager == null)
            {
                lock (syncRoot)
                {
                    if (registryManager == null)
                    {
                        MobileAppSettingsDictionary settings = this.Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();
                        string iotHubconnectionString = settings["IoTHubConnectionString"]; //ConfigurationManager.AppSettings["IoTHubConnectionString"];
                        registryManager = RegistryManager.CreateFromConnectionString(iotHubconnectionString);
                    }
                }
            }

            
            try
            {
                device = await registryManager.AddDeviceAsync(new Device(deviceName));
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await registryManager.GetDeviceAsync(deviceName);
            }
            return device.Authentication.SymmetricKey.PrimaryKey;
        }
    }
}
