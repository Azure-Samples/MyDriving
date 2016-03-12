using MyTrips.Interfaces;
#if WINDOWS_UWP
using ObdLibUWP;
#elif __ANDROID__
using ObdLibAndroid;
#elif __IOS__
using ObdLibiOS;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrips.Shared
{
    public class OBDDevice : IOBDDevice
    {
        ObdWrapper obdWrapper = new ObdWrapper();

        public async Task Disconnect()
        {
            await this.obdWrapper.Disconnect();
        }

        public async Task<bool> Initialize(bool simulatorMode = false)
        {
            return await this.obdWrapper.Init(simulatorMode);
        }

        public Dictionary<String, String> ReadData()
        {
            return this.obdWrapper.Read();
        }
    }
}
