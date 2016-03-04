﻿using MyTrips.AzureClient;
using MyTrips.DataObjects;
using MyTrips.DataStore.Abstractions;
using MyTrips.DataStore.Mock;
using MyTrips.Interfaces;
using MyTrips.Utils;
using MyTrips.ViewModel;
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
        int pushDataAttempts = 0;
        IHubIOT iotHub;
        IOBDDevice obdDevice;
        bool canReadData;
        Stopwatch obdReconnectTimer;
        //MyTrips.DataStore.Azure.StoreManager storeManager;
        MyTrips.DataStore.Mock.StoreManager storeManager;

        public delegate void OBDDeviceHandler(bool retryToConnect);
        public event OBDDeviceHandler OnOBDDeviceDisconnected;

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

            this.obdReconnectTimer = new Stopwatch();

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
            foreach (Trail tripDataPoint in currentTrip.Trail)
            {
                var blob = JsonConvert.SerializeObject(
                    new
                    {
                        Id = currentTrip.Id,
                        Name = currentTrip.TripId,
                        UserId = currentTrip.UserId,
                        TripDataPoint = JsonConvert.SerializeObject(tripDataPoint)
            }
                 );

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
                    await this.storeManager.IOTHubStore.DropTable();
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
                    //Give up after 24 hours???
                    this.OnOBDDeviceDisconnected(false);
                    this.canReadData = false;
                }
            }

            this.obdReconnectTimer.Stop();
            this.canReadData = true;
        }

        //TODO: Should be called by mobile app when an app is resumed
        public void ResetIncrementalConnection()
        {
            this.obdReconnectTimer.Restart();
        }
    }
}
