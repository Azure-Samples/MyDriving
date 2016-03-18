// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace IoTHubPartitionMap.Interfaces
{
    public interface IIoTHubPartitionMap : IActor
    {
        Task<string> LeaseTHubPartitionAsync();
        Task<string> RenewIoTHubPartitionLeaseAsync(string partition);
    }
}