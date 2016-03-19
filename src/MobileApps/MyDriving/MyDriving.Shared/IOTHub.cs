// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using MyDriving.Interfaces;

namespace MyDriving.Shared
{
    public class IOTHub : IHubIOT
    {
        private DeviceClient deviceClient;

        public void Initialize(string connectionStr)
        {
            deviceClient = DeviceClient.CreateFromConnectionString(connectionStr);
        }

        public async Task SendEvents(IEnumerable<String> blobs)
        {
            List<Message> messages = blobs.Select(b => new Message(Encoding.ASCII.GetBytes(b))).ToList();
            await deviceClient.SendEventBatchAsync(messages);
        }

        public async Task SendEvent(string blob)
        {
            var message = new Message(Encoding.ASCII.GetBytes(blob));
            await deviceClient.SendEventAsync(message);
        }
    }
}