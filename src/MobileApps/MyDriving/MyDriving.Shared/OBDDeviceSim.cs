using MyDriving.Interfaces;
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

namespace MyDriving.Shared
{
    public class OBDDeviceSim : IOBDDevice
    {
        ObdWrapper obdWrapper = new ObdWrapper();

        public async Task Disconnect()
        {
            await this.obdWrapper.Disconnect();
        }

        public async Task<bool> Initialize()
        {
            return await this.obdWrapper.Init(true);
        }

        public bool IsSimulated => true;

        public Dictionary<string, string> ReadData()
        {
            return this.obdWrapper.Read();
        }
    }
}
