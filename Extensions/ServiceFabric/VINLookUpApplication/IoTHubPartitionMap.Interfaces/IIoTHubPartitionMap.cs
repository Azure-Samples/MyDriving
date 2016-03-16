using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
