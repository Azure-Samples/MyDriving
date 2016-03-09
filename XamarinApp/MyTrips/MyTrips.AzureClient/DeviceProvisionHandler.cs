using Microsoft.WindowsAzure.MobileServices;
using MyTrips.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyTrips.AzureClient
{
    public class DeviceProvisionHandler
    {
        private static DeviceProvisionHandler handler;

        private const string defaultHostName = "smarttrips-dev.azure-devices.net";

        private DeviceProvisionHandler()
        {
            if (!string.IsNullOrEmpty(Settings.Current.DeviceId))
                DeviceId = Settings.Current.DeviceId;
            else
            {
                //generate device ID
                DeviceId = Guid.NewGuid().ToString();
                Settings.Current.DeviceId = DeviceId;
            }

            if (!string.IsNullOrEmpty(Settings.Current.HostName))
                HostName = Settings.Current.HostName;
            else
            {
                HostName = defaultHostName;
                Settings.Current.HostName = HostName;
            }
        }
        public string DeviceId
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
            }

            return handler;
        }

        public async Task<string> ProvisionDevice()
        {
            if (!string.IsNullOrEmpty(Settings.Current.DeviceConnectionString))
                return Settings.Current.DeviceConnectionString;
            else
            {
                Dictionary<string, string> myParms = new Dictionary<string, string>();
                myParms.Add("userId", Settings.Current.UserId);
                myParms.Add("deviceName", Settings.Current.DeviceId);

                var client = ServiceLocator.Instance.Resolve<IAzureClient>()?.Client as MobileServiceClient;

                try
                {
                    var response = await client.InvokeApiAsync("provision", null, HttpMethod.Post, myParms);
                    this.AccessKey = response.Value<string>();
                }
                catch (Exception e)
                {
                    Logger.Instance.WriteLine("Unable to provision device with IOT Hub: " + e);
                }

                Settings.Current.DeviceConnectionString = this.DeviceConnectionString;
                return Settings.Current.DeviceConnectionString;
            }
        }
    }
}
