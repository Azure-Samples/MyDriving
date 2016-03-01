using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrips.Interfaces
{
    public interface IOBDDevice
    {
        Task Initialize();

        bool IsReadingData { get; set; }

        Dictionary<String, String> ReadData();

        Task Disconnect();
    }
}
