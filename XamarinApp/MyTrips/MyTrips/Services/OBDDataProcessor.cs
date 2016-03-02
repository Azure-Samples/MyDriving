using MyTrips.DataStore.Azure;
using MyTrips.Interfaces;
using MyTrips.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrips.Services
{
    public class ODBDataProcessor
    {
        const int Interval = 3000;
        IHubIOT iotHub;
        IOBDDevice obdDevice;
        Dictionary<string, string> diagnosticDataDictionary;

        //Init must be called each time to connect and reconnect to the OBD device
        public async Task Init()
        {
            //Get platform specific implemenation of IOTHub and IOBDDevice
            this.iotHub = ServiceLocator.Instance.Resolve<IHubIOT>();
            this.obdDevice = ServiceLocator.Instance.Resolve<IOBDDevice>();

            //Call into mobile service to provision the device
            var connectionStr = await DeviceProvisionHandler.GetHandler().ProvisionDevice();

            //Initialize both the IOTHub and the OBD device to begin reading and processing data
            await this.iotHub.Initialize(connectionStr);
            await this.obdDevice.Initialize();

            this.obdDevice.IsReadingData = true;
        }

        public async Task ProcessDiagnosticData()
        {
            while (this.obdDevice.IsReadingData)
            {
                await Task.Delay(Interval);
                this.diagnosticDataDictionary = this.obdDevice.ReadData();

                if (this.diagnosticDataDictionary != null && this.DataIsRefreshed())
                {
                    //TODO: Need to package timestamp and GPS data with this
                    string diagnosticDataBlob = JsonConvert.SerializeObject(this.diagnosticDataDictionary);
                    await this.iotHub.SendEvent(diagnosticDataBlob);
                }
                else
                {
                    //Null is returned when OBD device cannot be connected to
                    this.obdDevice.IsReadingData = false;
                    break;
                }
            }
        }

        private bool DataIsRefreshed()
        {
            bool dataIsRefreshed = false;

            //If the dictionary contains all empty strings, then it hasn't been refreshed with new data yet
            foreach (var key in this.diagnosticDataDictionary.Keys)
            {
                if (this.diagnosticDataDictionary[key] != String.Empty)
                {
                    dataIsRefreshed = true;
                    break;
                }
            }

            return dataIsRefreshed;
        }
    }
}