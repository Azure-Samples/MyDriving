using MyTrips.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrips.iOS.Helpers
{
    public class OBDDeviceSim : IOBDDevice
    {
        //TODO: Uncomment when ODBLib is available for iOS
        //ObdWrapper obdWrapper = new ObdWrapper();

        public async Task Disconnect()
        {
            //await this.obdWrapper.Disconnect();
            throw new NotImplementedException();
        }

        public async Task Initialize()
        {
            //await this.obdWrapper.Init(true);
            throw new NotImplementedException();
        }

        public Dictionary<string, string> ReadData()
        {
            //return this.obdWrapper.Read();
            throw new NotImplementedException();
        }
    }
}
