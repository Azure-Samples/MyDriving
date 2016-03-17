using Microsoft.Azure.Devices.Client;
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
        private DeviceClient deviceClient;

        public void Initialize(string connectionStr)
        {
            this.deviceClient = DeviceClient.CreateFromConnectionString(connectionStr);
        }

        public async Task SendEvents(IEnumerable<String> blobs)
        {
            List<Microsoft.Azure.Devices.Client.Message> messages = blobs.Select(b => new Microsoft.Azure.Devices.Client.Message(UTF8Encoding.Unicode.GetBytes(b))).ToList();
            await this.deviceClient.SendEventBatchAsync(messages);
        }

        public async Task SendEvent(string blob)
        {
            var message = new Message(UTF8Encoding.Unicode.GetBytes(blob));
            await this.deviceClient.SendEventAsync(message);
        }
    }
}
