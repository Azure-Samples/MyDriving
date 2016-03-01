using Microsoft.Azure.Devices.Client;
using MyTrips.Interfaces;
using ObdLibUWP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrips.UWP.Helpers
{
    public class IOTHub
    {
        private DeviceClient deviceClient { get; set; }

        public void InitializeServiceAsync(string connectionStr)
        {
            this.deviceClient = DeviceClient.CreateFromConnectionString(connectionStr);
        }

        public async Task<bool> SendEventAsync(string jsonBlob)
        {
            var message = new Message(Encoding.ASCII.GetBytes(jsonBlob));

            try
            {
                await this.deviceClient.SendEventAsync(message);
            }
            catch(Exception e)
            {
                //TODO: need to add retry logic
                throw;
            }

            return true;
        }
    }
}
