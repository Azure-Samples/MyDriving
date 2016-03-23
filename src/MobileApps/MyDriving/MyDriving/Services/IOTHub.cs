// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using MyDriving.Interfaces;

namespace MyDriving.Services
{
    public class IOTHub : IHubIOT
    {
        private DeviceClient deviceClient;

        public void Initialize(string connectionStr)
        {
            if (string.IsNullOrWhiteSpace(connectionStr))
                return;

            deviceClient = DeviceClient.CreateFromConnectionString(connectionStr, TransportType.Http1);
        }

        public async Task SendEvents(IEnumerable<String> blobs)
        {
            if (deviceClient == null)
                return;

            List<Message> messages = blobs.Select(b => new Message(Encoding.UTF8.GetBytes(b))).ToList();
            await deviceClient.SendEventBatchAsync(messages);
        }

        public async Task SendEvent(string blob)
        {
            if (deviceClient == null)
                return;

            var message = new Message(Encoding.UTF8.GetBytes(blob));
            await deviceClient.SendEventAsync(message);
        }
    }
}