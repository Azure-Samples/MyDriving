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

        public async Task SendEvents(IEnumerable<String> blobs)
        {
            List<Microsoft.Azure.Devices.Client.Message> messages = blobs.Select(b => new Microsoft.Azure.Devices.Client.Message(System.Text.Encoding.ASCII.GetBytes(b))).ToList();
            await this.deviceClient.SendEventBatchAsync(messages);
        }
#else
        public void Initialize(string connectionStr)
        {
           
        }

        public Task SendEvents(IEnumerable<String> blobs)
        {
            return Task.FromResult(true);
        }
        #endif
    }
}
