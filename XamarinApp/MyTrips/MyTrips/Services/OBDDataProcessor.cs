using MyTrips.AzureClient;
using MyTrips.DataObjects;
using MyTrips.DataStore.Abstractions;
using MyTrips.DataStore.Mock;
using MyTrips.Interfaces;
using MyTrips.Utils;
using MyTrips.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace MyTrips.Services
{
    public class OBDDataProcessor
    {
        int pushDataAttempts = 0;
        IHubIOT iotHub;
        IOBDDevice obdDevice;
        bool canReadData;
        Stopwatch obdReconnectTimer;
        IStoreManager storeManager;

        public delegate void OBDDeviceHandler(bool retryToConnect);
        public event OBDDeviceHandler OnOBDDeviceDisconnected;

        //Init must be called each time to connect and reconnect to the OBD device
        public async Task Initialize(IStoreManager storeManager)
        {
            this.obdReconnectTimer = new Stopwatch();
            this.storeManager = storeManager;

            //Get platform specific implemenation of IOTHub and IOBDDevice
            this.iotHub = ServiceLocator.Instance.Resolve<IHubIOT>();
            this.obdDevice = ServiceLocator.Instance.Resolve<IOBDDevice>();

            //Call into mobile service to provision the device
 
            var connectionStr = Settings.Current.DeviceConnectionString;
            if (string.IsNullOrEmpty(connectionStr))
            {
                //connectionStr = await DeviceProvisionHandler.GetHandler().ProvisionDevice();

                //Hack for bug #320
                connectionStr = DeviceProvisionHandler.GetHandler().DeviceConnectionString;
                //When bug #319 is fixed, we should remove this and uncomment the above line
            }

            Settings.Current.DeviceConnectionString = connectionStr;
            //Initialize the IOT Hub
            this.iotHub.Initialize(connectionStr);

            CrossConnectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
        }

        public Dictionary<String, String> ReadOBDData()
        {
            Dictionary<String, String> obdData;
            if (this.canReadData)
            {
                obdData = this.obdDevice.ReadData();

                if (obdData == null)
                {
                    //Null is returned if connection to the OBD device is dropped
                    this.canReadData = false;
                    this.OnOBDDeviceDisconnected(true);
                    obdData = new Dictionary<string, string>();
                }
            }
                else
                {
                    obdData = new Dictionary<string, string>();
                }

            return obdData;
        }

        public async Task AddTripDataPointToBuffer(Trip currentTrip)
        {
            //Note: Each individual trip point is being serialized separately so that it can be sent over as an individual message
            //This was the requested format by Haishi's team
            foreach (var tripDataPoint in currentTrip.Points)
            {
                var settings = new JsonSerializerSettings();
                settings.ContractResolver = new CustomContractResolver();
                var tripDataBlob = JsonConvert.SerializeObject(tripDataPoint, settings);

                var blob = JsonConvert.SerializeObject(
                    new
                    {
                        TripId = currentTrip.Id,
                        Name = currentTrip.Name,
                        UserId = currentTrip.UserId,
                        TripDataPoint = tripDataBlob
                    } );

                //Remove extra quotes in trip point
                blob = blob.Replace(":\"{", ":{");
                blob = blob.Replace("}\"}", "}}");

                IOTHubData iotHubData = new IOTHubData();
                iotHubData.Blob = blob;
                await this.storeManager.IOTHubStore.InsertAsync(iotHubData);
            }
        }

        private async void Current_ConnectivityChanged(object sender, Plugin.Connectivity.Abstractions.ConnectivityChangedEventArgs e)
        {
            if (e.IsConnected)
            {
                await this.PushTripDataToIOTHub();
            }
        }

        public async Task PushTripDataToIOTHub()
        {
            pushDataAttempts++;
            var iotHubDataBlobs = await this.storeManager.IOTHubStore.GetItemsAsync();

            //Stop pushing data if the buffer is empty (which means all data has been successfully pushed)
            //Or, we've had more than 50 failed attempts
            if (iotHubDataBlobs.Count() <= 0 || pushDataAttempts > 50)
            {
                pushDataAttempts = 0;
                return;
            }

            if (CrossConnectivity.Current.IsConnected)
            {
                try
                {
                    //Once the trip is pushed to the IOT Hub, delete it from the local store
                    await this.iotHub.SendEvents(iotHubDataBlobs.Select(i => i.Blob));
                    await this.storeManager.IOTHubStore.RemoveItemsAsync(iotHubDataBlobs);
                }
                catch (Exception ex)
                {
                    //An exception will be thrown if the data isn't received by the IOT Hub
                    await Task.Delay(1000);
                    Logger.Instance.Report(ex);
                }
            }
            else
            {
                //If there is no network connection, then stop trying to push data entirely
                //Instead, we'll wait to try to push data again when the ConnectivityChanged event is raised with successful network connection
                return;
            }

            //If any data wasn't received by the IOT Hub, there may still be data in the local store - try again
            await this.PushTripDataToIOTHub();
        }

        public async Task DisconnectFromOBDDevice()
        {
            await this.obdDevice.Disconnect();
            this.canReadData = false;
        }

        public async Task ConnectToOBDDevice()
        {
            this.obdReconnectTimer.Restart();
            while (!await this.obdDevice.Initialize())
            {
                if (this.obdReconnectTimer.Elapsed.Minutes <= 5)
                {
                    //Try to connect every 10 seconds if the OBD device disconnected time is 5 mins or less
                    await Task.Delay(10000);
                }
                else if (this.obdReconnectTimer.Elapsed.Minutes <= 30)
                {
                    //Try to connect every 5 minutes if the OBD device disconnected time is 30 mins or less
                    await Task.Delay(new TimeSpan(0, 5, 0));
                }
                else if (this.obdReconnectTimer.Elapsed.Hours <= 24)
                {
                    //Otherwise, try to connect every 30 minutes
                    await Task.Delay(new TimeSpan(0, 30, 0));
                }
                else
                {
                    //Give up after 24 hours
                    this.OnOBDDeviceDisconnected(false);
                    this.obdReconnectTimer.Stop();
                    this.canReadData = false;
                    return;
                }
            }

            this.obdReconnectTimer.Stop();
            this.canReadData = true;
        }

        //TODO: Should be called by mobile app when an app is resumed
        public void ResetIncrementalConnection()
        {
			if (obdReconnectTimer != null)
			{
				obdReconnectTimer.Restart();
			}
        }
    }

    public class CustomContractResolver : DefaultContractResolver
    {
        private Dictionary<string, string> PropertyMappings { get; set; }

        private List<string> IgnoreProperties { get; set; }

        public CustomContractResolver()
        {
            this.PropertyMappings = new Dictionary<string, string>();
            this.PropertyMappings.Add("Longitude", "Lon");
            this.PropertyMappings.Add("Latitude", "Lat");
            this.PropertyMappings.Add("ShortTermFuelBank", "ShortTermFuelBank1");
            this.PropertyMappings.Add("LongTermFuelBank", "LongTermFuelBank1");
            this.PropertyMappings.Add("MassFlowRate", "MAFFlowRate");
            this.PropertyMappings.Add("RPM", "EngineRPM");
            this.PropertyMappings.Add("Id", "TripPointId");
            this.PropertyMappings.Add("DistanceWithMalfunctionLight", "DistancewithMIL");

            this.IgnoreProperties = new List<string>();
            this.IgnoreProperties.Add("HasOBDData");
        }

        protected override string ResolvePropertyName(string propertyName)
        {
            string resolvedName = null;
            var resolved = this.PropertyMappings.TryGetValue(propertyName, out resolvedName);
            return (resolved) ? resolvedName : base.ResolvePropertyName(propertyName);
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (this.IgnoreProperties.Contains(property.PropertyName))
            {
                property.ShouldSerialize = p => { return false; };
            }

            return property;
        }
    }
}