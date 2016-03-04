using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrips.Interfaces
{
    public interface IOBDDevice
    {
        Task<bool> Initialize();

        Dictionary<String, String> ReadData();

        Task Disconnect();
    }
}
