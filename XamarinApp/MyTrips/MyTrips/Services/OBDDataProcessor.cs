using MyTrips.AzureClient;
using MyTrips.DataObjects;
using MyTrips.DataStore.Abstractions;
using MyTrips.DataStore.Mock;
using MyTrips.Interfaces;
using MyTrips.Utils;
using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrips.Services
{
    public class OBDDataProcessor
    {
        IHubIOT iotHub;
        IOBDDevice obdDevice;
        Dictionary<string, string> diagnosticDataDictionary;
        bool canReadData;
        StringBuilder obdDataBuffer;
        Stopwatch obdReconnectTimer;
        //MyTrips.DataStore.Azure.StoreManager storeManager;
        MyTrips.DataStore.Mock.StoreManager storeManager;

        public event EventHandler OnOBDDeviceDisconnected;

        //Init must be called each time to connect and reconnect to the OBD device
        public async Task Initialize()
        {
            //Get platform specific implemenation of IOTHub and IOBDDevice
            this.iotHub = ServiceLocator.Instance.Resolve<IHubIOT>();
            this.obdDevice = ServiceLocator.Instance.Resolve<IOBDDevice>();

            //TODO: Need to add compiler dir for debug
            this.storeManager = ServiceLocator.Instance.Resolve<IStoreManager>() as MyTrips.DataStore.Mock.StoreManager;
            //this.storeManager = ServiceLocator.Instance.Resolve<IStoreManager>() as MyTrips.DataStore.Azure.StoreManager;

            //Call into mobile service to provision the device
            var connectionStr = await DeviceProvisionHandler.GetHandler().ProvisionDevice();

            //Initialize the IOT Hub
            this.iotHub.Initialize(connectionStr);

            this.obdDataBuffer = new StringBuilder();
            this.obdReconnectTimer = new Stopwatch();

            CrossConnectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
        }

        public async Task StartReadingOBDData()
        {
            this.canReadData = await this.ConnectToOBDDevice();

            while (this.canReadData)
            {
                this.diagnosticDataDictionary = this.obdDevice.ReadData();

                //If the dictionary contains all empty strings, then it hasn't been refreshed with new data yet
                bool isDataRefreshed = this.diagnosticDataDictionary.Values.Where(d => d != String.Empty).ToArray().Count() >= 1;

                //Null is returned when OBD device cannot be connected to
                if (this.diagnosticDataDictionary != null && isDataRefreshed)
                {
                    //Buffer the trip data read from the OBD device
                    string obdDataPoint = JsonConvert.SerializeObject(this.diagnosticDataDictionary);
                    this.obdDataBuffer.Append(obdDataPoint);
                }
                else
                {
                    this.canReadData = false;
                    this.canReadData = await this.ConnectToOBDDevice();
                }

                //Attempt to read OBD device data every 3 seconds
                await Task.Delay(3000);
            }
        }

        private async void Current_ConnectivityChanged(object sender, Plugin.Connectivity.Abstractions.ConnectivityChangedEventArgs e)
        {
            if (e.IsConnected)
            {
                await this.PushTripData();
            }
        }

        public async Task PushTripData(Trip currentTrip)
        {
            //TODO: Still deciding on data that will be sent and format - just packaging a bunch of stuff for now
            //Package the trip data gathered from the phone
            IOTHubData iotHubData = new IOTHubData();
            iotHubData.Id = currentTrip.Id;
            iotHubData.TripName = currentTrip.Name;
            iotHubData.UserId = currentTrip.UserId;
            iotHubData.TimeStamp = currentTrip.TimeStamp;
            iotHubData.TripPoints = JsonConvert.SerializeObject(currentTrip.Points);

            //And package it with the OBD data
            iotHubData.OBDData = this.obdDataBuffer.ToString();
            this.obdDataBuffer.Clear();

            //Store the trip data locally until it's successfully pushed to the IOT Hub
            await this.storeManager.IOTHubStore.InsertAsync(iotHubData);

            await this.PushTripData();
        }

        private async Task PushTripData()
        {
            var iotHubDataBlobs = await this.storeManager.IOTHubStore.GetItemsAsync();

            if (iotHubDataBlobs.Count() <= 0)
            {
                return;
            }

            foreach (var blob in iotHubDataBlobs)
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    try
                    {
                        //Once the trip is pushed to the IOT Hub, delete it from the local store
                        await this.iotHub.SendEvent(JsonConvert.SerializeObject(blob));
                        await this.storeManager.IOTHubStore.RemoveAsync(blob);
                    }
                    catch (Exception e)
                    {
                        //An exception will be thrown if the data isn't received by the IOT Hub
                        //In this case, wait a second and try again with the next trip
                        await Task.Delay(1000);
                    }
                }
                else
                {
                    //If there is no network connection, then stop trying to push data entirely
                    //Instead, we'll wait to try to push data again when the ConnectivityChanged event is raised with successful network connection
                    return;
                }
            }

            //If any data wasn't received by the IOT Hub, there may still be data in the local store - try again
            await this.PushTripData();
        }

        public async Task StopReadingOBDData()
        {
            await this.obdDevice.Disconnect();
            this.canReadData = false;
        }

        private async Task<bool> ConnectToOBDDevice()
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
                    //Give up after 24 hours???
                    this.OnOBDDeviceDisconnected(this.obdDevice, new EventArgs());
                    return false;
                }
            }

            this.obdReconnectTimer.Stop();
            return true;
        }

        //Called by mobile app when an app is resumed
        public void ResetIncrementalConnection()
        {
            this.obdReconnectTimer.Restart();
        }
    }
}
