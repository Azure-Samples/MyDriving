#if WINDOWS_UWP
using Microsoft.Azure.Devices.Client;
#endif
using MyTrips.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrips.Shared
{
    public class IOTHub : IHubIOT
    {
        #if WINDOWS_UWP
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
        #else
        public void Initialize(string connectionStr)
        {
           
        }

        public Task SendEvent(string blob)
        {
            return Task.FromResult(true);
        }
        #endif
    }
}
