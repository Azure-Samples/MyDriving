// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using MyDriving.DataObjects;
using MyDriving.DataStore.Abstractions;
using System.Threading.Tasks;

namespace MyDriving.DataStore.Azure.Stores
{
    public class IOTHubStore : BaseStore<IOTHubData>, IHubIOTStore
    {
        public override string Identifier => "IOTHub";

        public override Task<bool> SyncAsync()
        {
            return Task.FromResult(true);
        }

        object locker = new object();
        public override Task<bool> InsertAsync(IOTHubData item)
        {
            lock(locker)
            {
                return base.InsertAsync(item);
            }
        }
    }
}