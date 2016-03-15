using MyTrips.AzureClient;
using MyTrips.DataObjects;
using MyTrips.DataStore.Abstractions;
using MyTrips.Interfaces;
using MyTrips.Utils;
using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTrips.Services
{
    public class OBDDataProcessor
    {
        int pushDataAttempts = 0;
        IHubIOT iotHub;
        IStoreManager storeManager;

        //OBD Device state
        IOBDDevice obdDevice;
        Timer obdConnectionTimer;
        TimeSpan obdEllapsedTime;
        bool isConnectedToOBD;
        bool isPollingOBDDevice;

        //Init must be called each time to connect and reconnect to the OBD device
        public async Task Initialize(IStoreManager storeManager)
        {
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

        public async Task<Dictionary<String, String>> ReadOBDData()
        {
            Dictionary<String, String> obdData = null;
            if (this.isConnectedToOBD)
            {
                //Null is returned if connection to the OBD device is dropped
                obdData = this.obdDevice.ReadData();

                if (obdData == null)
                {
                    //Invoke in background so that caller isn't blocked while trying to connect to OBD device
                    this.isConnectedToOBD = false;
                    this.ConnectToOBDDevice(false); 
                }
            }

            return obdData;
        }

        public async Task AddTripDataPointToBuffer(Trip currentTrip)
        {
            //Note: Each individual trip point is being serialized separately so that it can be sent over as an individual message
            //This is the expected format by the IOT Hub\ML
            foreach (var tripDataPoint in currentTrip.Points)
            {
                var settings = new JsonSerializerSettings();
                settings.ContractResolver = new CustomContractResolver();
                var tripDataPointBlob = JsonConvert.SerializeObject(tripDataPoint, settings);

                var tripBlob = JsonConvert.SerializeObject(
                    new
                    {
                        TripId = currentTrip.Id,
                        Name = currentTrip.Name,
                        UserId = currentTrip.UserId
                    });

                tripBlob = tripBlob.TrimEnd('}');
                var packagedBlob = String.Format("{0},\"TripDataPoint\":{1}}}", tripBlob, tripDataPointBlob);

                IOTHubData iotHubData = new IOTHubData();
                iotHubData.Blob = packagedBlob;
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
            //Stop polling the device in case we're still trying to connect to it
            this.StopPollingOBDDevice();

            //Disconnect to the device in the case that we did successfull connect to it
            await this.obdDevice.Disconnect();

            this.isConnectedToOBD = false;
        }

        public async Task ConnectToOBDDevice(bool showConfirmDialog)
        {
            if (showConfirmDialog)
            {
                //Prompts user with dialog to retry if connection to OBD device fails
                this.isConnectedToOBD = await this.ConnectToOBDDeviceWithConfirmation();
            }
            else
            {
                //Silently attempts to connect to the OBD device
                this.StartPollingOBDDevice();
            }
        }

        private async Task<bool> ConnectToOBDDeviceWithConfirmation()
        {
            bool isConnected = await this.obdDevice.Initialize();

            if (!isConnected)
            {
                var result = await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync("Unable to connect to OBD device.  Would you like to attempt to connect to the OBD device again?",
                    "OBD Connection", "Retry", "Use Simulator");

                if (result)
                {
                    //Attempt to connect to OBD device again
                    isConnected = await this.ConnectToOBDDeviceWithConfirmation();
                }
                else
                {
                    //Use the OBD simulator
                    isConnected = await this.obdDevice.Initialize(true);
                }
            }

            return isConnected;
        }

        private void StartPollingOBDDevice()
        {
            if (!this.isConnectedToOBD && !this.isPollingOBDDevice)
            {
                this.isPollingOBDDevice = true;
                this.obdEllapsedTime = new TimeSpan(0, 0, 0);
                this.obdConnectionTimer = new Timer(OnTick, 0, 1000, 1000);
            }
        }

        private void StopPollingOBDDevice()
        {
            if (this.isPollingOBDDevice)
            {
                this.obdConnectionTimer.Cancel();
                this.obdConnectionTimer.Dispose();
                this.isPollingOBDDevice = false;
                this.obdEllapsedTime = new TimeSpan(0, 0, 0);
            }
        }

        private async Task TryToConnectToOBDDevice()
        {
            if (this.isConnectedToOBD = await this.obdDevice.Initialize())
            {
                this.StopPollingOBDDevice();
            }
        }

        private async void OnTick(object args)
        {
            this.obdEllapsedTime = this.obdEllapsedTime.Add(new TimeSpan(0, 0, 1));

            if (this.obdEllapsedTime.TotalMinutes < 5 && this.obdEllapsedTime.TotalSeconds % 10 == 0)
            {
                //Try to connect every 10 seconds if the OBD device disconnected time is 5 mins or less
                await this.TryToConnectToOBDDevice();
            }
            else if (this.obdEllapsedTime.TotalMinutes < 30 && this.obdEllapsedTime.TotalMinutes % 5 == 0)
            {
                //Try to connect every 5 minutes if the OBD device disconnected time is 30 mins or less
                await this.TryToConnectToOBDDevice();
            }
            else if (this.obdEllapsedTime.TotalHours < 3 && this.obdEllapsedTime.TotalMinutes % 30 == 0)
            {
                //Otherwise, try to connect every 30 minutes
                await this.TryToConnectToOBDDevice();
            }
            else if (this.obdEllapsedTime.TotalHours >= 3)
            {
                //Give up after 3 hours
                this.StopPollingOBDDevice();
            }
        }
    }
}

   