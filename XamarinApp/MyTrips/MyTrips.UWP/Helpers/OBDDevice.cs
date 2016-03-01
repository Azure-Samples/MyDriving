using MyTrips.Interfaces;
using ObdLibUWP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrips.UWP
{
    public class OBDDevice : IOBDDevice
    {
        ObdWrapper obdWrapper = new ObdWrapper();

        public bool IsReadingData
        {
            get; set;
        }

        public async Task Disconnect()
        {
            await this.obdWrapper.Disconnect();
        }

        public async Task Initialize()
        {
            await this.obdWrapper.Init();
        }

        public Dictionary<String, String> ReadData()
        {
            return this.obdWrapper.Read();
        }
    }
}
