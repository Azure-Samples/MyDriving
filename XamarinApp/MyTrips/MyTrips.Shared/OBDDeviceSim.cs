using MyTrips.Interfaces;
#if WINDOWS_UWP
using ObdLibUWP;
#elif __ANDROID__
using ObdLibAndroid;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrips.Shared
{
    public class OBDDeviceSim : IOBDDevice
    {
        #if WINDOWS_UWP || __ANDROID__
        ObdWrapper obdWrapper = new ObdWrapper();

        public async Task Disconnect()
        {
            await this.obdWrapper.Disconnect();
        }

        public async Task<bool> Initialize()
        {
            return await this.obdWrapper.Init(true);
        }

        public Dictionary<string, string> ReadData()
        {
            return this.obdWrapper.Read();
        }
#else
        public Task Disconnect()
        {
            return Task.FromResult(true);
        }

        public Task<bool> Initialize()
        {
            return Task.FromResult(true);
        }

        public Dictionary<string, string> ReadData()
        {
            return new Dictionary<string, string>();
        }
        #endif
    }
}
