using MyTrips.Interfaces;
using MyTrips.UWP.Helpers;
using ObdLibUWP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrips.UWP
{
    public class OBDDeviceSim : IOBDDevice
    {
        ObdWrapper obdWrapper = new ObdWrapper();

        public async Task Disconnect()
        {
            await this.obdWrapper.Disconnect();
        }

        public async Task Initialize()
        {
            await this.obdWrapper.Init(true);
        }

        public Dictionary<string, string> ReadData()
        {
            return this.obdWrapper.Read();
        }
    }
}
