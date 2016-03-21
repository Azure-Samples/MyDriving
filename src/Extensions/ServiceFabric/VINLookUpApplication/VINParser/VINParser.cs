// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace VINParser
{
    public class VINParser
    {
        private const string MYearMap = "ABCDEFGHJKLMNPRSTVWXY123456789";
        private readonly VMIProvider mVmiProvider;

        public VINParser()
        {
            mVmiProvider = new VMIProvider();
        }

        public VINParser(string vmiJsonFile)
        {
            mVmiProvider = new VMIProvider(vmiJsonFile);
        }

        public CarInfo Parse(string vin)
        {
            if (vin == null || vin.Trim().Length != 17)
                throw new ArgumentException("VIN number has to be 17 digits long.");
            CarInfo ret = mVmiProvider.GetCarInfo(vin.Trim());
            ret.Year = GetYear(vin);
            return ret;
        }

        private int GetYear(string vin)
        {
            char yearDigit = vin[9];
            if (yearDigit == '0')
                return 1980; //Ford and AMC uses '0' for 1980
            int index = MYearMap.IndexOf(yearDigit);
            if (index < 0)
                throw new ArgumentNullException($"Unrecognized year digit: {yearDigit}");
            int year = 1980 + index;
            int temp;
            if (!int.TryParse(vin[6].ToString(), out temp))
                year += 30;
            return year;
        }
    }
}