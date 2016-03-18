using MyDriving.Interfaces;
using MyDriving.UWP.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDriving.UWP.Assets
{
    public class OBDDeviceSampleData : IOBDDevice
    {
        public bool IsReadingData
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public Task Disconnect()
        {
            throw new NotImplementedException();
        }

        public Task Initialize()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> ReadData()
        {
            throw new NotImplementedException();
        }
    }
}
