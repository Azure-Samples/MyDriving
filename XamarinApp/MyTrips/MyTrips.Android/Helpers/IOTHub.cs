using Microsoft.Azure.Devices.Client;
using MyTrips.Interfaces;
using ObdLibUWP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrips.Droid.Helpers
{
    public class IOTHub : IHubIOT
    {
        private DeviceClient deviceClient;

        public void Initialize(string connectionStr)
        {
            this.deviceClient = DeviceClient.CreateFromConnectionString(connectionStr);
        }

        public async Task SendEvent(string blob)
        {
            var message = new Message(Encoding.ASCII.GetBytes(blob));

            try
            {
                await this.deviceClient.SendEventAsync(message);
            }
            catch
            {
                //TODO: need to add retry logic
                throw;
            }
        }
    }
}
