// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.WindowsAzure.MobileServices;
using MyDriving.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyDriving.AzureClient
{
    public class DeviceProvisionHandler
    {
        private const string DefaultHostName = "mydriving.azure-devices.net";
        private static DeviceProvisionHandler _handler;

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
                HostName = DefaultHostName;
                Settings.Current.HostName = HostName;
            }
        }

        public string DeviceId { get; private set; }

        public string HostName { get; private set; }

        public string AccessKey { get; private set; }

        public string DeviceConnectionString
        {
            get
            {
                string connectionStr = String.Empty;
                if (!String.IsNullOrEmpty(AccessKey))
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
                return Settings.Current.DeviceConnectionString;
            else
            {
                Dictionary<string, string> myParms = new Dictionary<string, string>
                {
                    {"userId", Settings.Current.UserUID},
                    {"deviceName", Settings.Current.DeviceId}
                };

                var client = ServiceLocator.Instance.Resolve<IAzureClient>()?.Client as MobileServiceClient;

                try
                {
                    var response = await client.InvokeApiAsync("provision", null, HttpMethod.Post, myParms);
                    AccessKey = response.Value<string>();
                }
                catch (Exception e)
                {
                    Logger.Instance.WriteLine("Unable to provision device with IOT Hub: " + e.Message);
                }

                Settings.Current.DeviceConnectionString = DeviceConnectionString;
                return Settings.Current.DeviceConnectionString;
            }
        }
    }
}