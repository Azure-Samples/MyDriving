using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VINParser
{
    public class VINParser
    {
        private VMIProvider mVMIProvider;

        public VINParser()
        {
            mVMIProvider = new VMIProvider();
        }
        public VINParser(string vmiJsonFile)
        {
            mVMIProvider = new VMIProvider(vmiJsonFile);
        }
        private const string mYearMap = "ABCDEFGHJKLMNPRSTVWXY123456789";
        public CarInfo Parse(string vin)
        {
            if (vin == null || vin.Length != 17)
                throw new ArgumentException("VIN number has to be 17 digits long.");
            CarInfo ret = mVMIProvider.GetCarInfo(vin);
            ret.Year = GetYear(vin);
            return ret;
        }
        private int GetYear(string vin)
        {
            char yearDigit = vin[9];
            if (yearDigit == '0')
                return 1980; //Ford and AMC uses '0' for 1980
            int index = mYearMap.IndexOf(yearDigit);
            if (index < 0)
                throw new ArgumentNullException(string.Format("Unrecognized year digit: {0}", yearDigit));
            int year = 1980 + index;
            int temp = 0;
            if (!int.TryParse(vin[6].ToString(), out temp))
                year += 30;
            return year;
        }
    }
}
