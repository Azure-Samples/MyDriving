// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.WindowsAzure.MobileServices;
using MyDriving.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Plugin.DeviceInfo;
using System.Text;

namespace MyDriving.AzureClient
{
    public class DeviceProvisionHandler
    {
        private const string DefaultHostName = "mydriving.azure-devices.net";
        private static DeviceProvisionHandler _handler;

        private DeviceProvisionHandler()
        {
            try
            {
                DeviceId = Settings.Current.DeviceId;
                HostName = Settings.Current.HostName;
            }
            catch (Exception e)
            {
                //Occassionally, a System.IO.FileLoadException can occur
                Logger.Instance.Track("Unable to get device id and/or host name from user settings: " + e.Message);
            }

            if (String.IsNullOrEmpty(DeviceId))
            {
                //generate device ID
                DeviceId = GenerateDeviceId();
                Settings.Current.DeviceId = DeviceId;
            }

            if (String.IsNullOrEmpty(HostName))
            {
                HostName = DefaultHostName;
                Settings.Current.HostName = HostName;
            }
        }

        private string GenerateDeviceId()
        {
            string id = CrossDeviceInfo.Current.Id;

            if (id == null)
                return id;

            int limit = 128;
            //remove unaccepted characters  - see https://azure.microsoft.com/en-us/documentation/articles/iot-hub-devguide/#device-identity-registry 
            //Note due to bug in Microsoft.Azure.Devices.Client.PCL some characters such as '+' in DeviceID still cause trouble

            StringBuilder sb = new StringBuilder(id.Length);

            foreach (char c in id)
            {
                if (Char.IsLetterOrDigit(c))
                    sb.Append(c);
                if (sb.Length >= limit)
                    break;
            }
            return sb.ToString();
        }

        public string DeviceId { get; private set; }

        public string HostName { get; private set; }

        public string AccessKey { get; private set; }

        public string DeviceConnectionString
        {
            get
            {
                string connectionStr = String.Empty;
                if (!String.IsNullOrEmpty(AccessKey) && !String.IsNullOrEmpty(HostName) && !String.IsNullOrEmpty(DeviceId))
                {
                    connectionStr = $"HostName={HostName};DeviceId={DeviceId};SharedAccessKey={AccessKey}";
                }

                return connectionStr;
            }
        }

        public static DeviceProvisionHandler GetHandler()
        {
            return _handler ?? (_handler = new DeviceProvisionHandler());
        }

        public async Task<string> ProvisionDevice()
        {
            if (!string.IsNullOrEmpty(Settings.Current.DeviceConnectionString))
            {
                return Settings.Current.DeviceConnectionString;
            }
            else
            {
                Dictionary<string, string> myParms = new Dictionary<string, string>();
                myParms.Add("userId", Settings.Current.UserUID);
                myParms.Add("deviceName", DeviceId);

                var client = ServiceLocator.Instance.Resolve<IAzureClient>()?.Client as MobileServiceClient;

                try
                {
                    var response = await client.InvokeApiAsync("provision", null, HttpMethod.Post, myParms);
                    AccessKey = response.Value<string>();
                }
                catch (Exception e)
                {
                    Logger.Instance.Track("Unable to provision device with IOT Hub: " + e.Message);
                }

                Settings.Current.DeviceConnectionString = DeviceConnectionString;
                return Settings.Current.DeviceConnectionString;
            }
        }
    }
}