using MyTrips.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrips.Droid.Helpers
{
    public class OBDDevice : IOBDDevice
    {
        //TODO: Uncomment when ODBLib is available for Android
        //ObdWrapper obdWrapper = new ObdWrapper();

        public async Task Disconnect()
        {
            // await this.obdWrapper.Disconnect();
            throw new NotImplementedException();
        }

        public async Task Initialize()
        {
            //await this.obdWrapper.Init();
            throw new NotImplementedException();
        }

        public Dictionary<String, String> ReadData()
        {
            //return this.obdWrapper.Read();
            throw new NotImplementedException();
        }
    }
}
