using Microsoft.WindowsAzure.MobileServices;
using MyTrips.AzureClient;
using MyTrips.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyTrips.DataStore.Azure
{
    /// <summary>
    /// The functionality in this class is for provisioning a device for use with the IOT Hub; this code is only applicable
    /// within the context of when the app is connected to an azure backend.  In other words, this functionality isn't ever called
    /// if you are running the app offline.
    /// </summary>
    public class DeviceProvisionHandler
    {
        private static DeviceProvisionHandler handler;

        public string DeviceId
        {
            get; private set;
        }

        public string UserId
        {
            get; private set;
        }

        public string HostName
        {
            get; private set;
        }

        public string AccessKey
        {
            get; private set;
        }

        public string DeviceConnectionString
        {
            get
            {
                string connectionStr = String.Empty;
                if (!String.IsNullOrEmpty(this.AccessKey))
                {
                    connectionStr = String.Format("HostName={0};DeviceId={1};SharedAccessKey={2}", this.HostName, this.DeviceId, this.AccessKey);
                }

                return connectionStr;
            }
        }

        public static DeviceProvisionHandler GetHandler()
        {
            if (handler == null)
            {
                handler = new DeviceProvisionHandler();
                //TODO: Need to get these values from Settings.Current
                handler.UserId = "TestDeviceUser";
                handler.DeviceId = "TestDevice";
                handler.HostName = "smarttrips-dev.azure-devices.net";
            }

            return handler;
        }

        public async Task<string> ProvisionDevice()
        {
            //TODO: Need to get these values from Settings.Current
            Dictionary<string, string> myParms = new Dictionary<string, string>();
            myParms.Add("userId", this.UserId);
            myParms.Add("deviceName", this.DeviceId);

            var response = await ServiceLocator.Instance.Resolve<IAzureClient>().Client.InvokeApiAsync("provision", null, HttpMethod.Post, myParms);
            this.AccessKey = response.Value<string>();
            return this.DeviceConnectionString;
        }
    }
}