// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Globalization;

namespace ObdShare
{
    public class ObdUtil
    {
        static string _outsideTemperature = "0";
        static double _distancewithMl;
        static int _runtime;

        private static int ParseString(string str, int bytes)
        {
            return int.Parse(str.Substring(4, bytes * 2), NumberStyles.HexNumber);
        }

        public static string ParseObd01Msg(string input)
        {
            string str = input.Replace("\r", "").Replace("\n", "").Replace(" ", "").Replace(">", "").Trim();
            if (!str.StartsWith("41") || str.Length < 6)
                return "-255";
            string pid = str.Substring(2, 2);
            int result;
            switch (pid)
            {
                case "04": //EngineLoad
                    return (ParseString(str, 1) * 100 / 255).ToString();
                case "06": //ShortTermFuelBank1
                    return ((ParseString(str, 1) - 128) * 100 / 128).ToString();
                case "07": //LongTermFuelBank1
                    return ((ParseString(str, 1) - 128) * 100 / 128).ToString();
                case "0C": //RPM
                    result = (ParseString(str, 2) / 4);
                    if (result < 0 || result > 16383)
                        result = -255;
                    return result.ToString();
                case "0D": //Speed
                    result = ParseString(str, 1);
                    if (result < 0 || result > 255)
                        result = -255;
                    return result.ToString();
                case "0F": //InsideTemperature
                    return (ParseString(str, 1) - 40).ToString();
                case "10": //MAF air flow rate
                    result = (ParseString(str, 2) / 100);
                    if (result < 0 || result > 655)
                        result = -255;
                    return result.ToString();
                case "11": //Throttle position
                    return (ParseString(str, 1) * 100 / 255).ToString();
                case "1F": //Runtime 
                    return ParseString(str, 2).ToString();
                case "21": //DistancewithML  
                    return ParseString(str, 2).ToString();
                case "2C": //Commanded EGR
                    return (ParseString(str, 1) * 100 / 255).ToString();
                case "2D": //EGR Error
                    return ((ParseString(str, 1) - 128) * 100 / 128).ToString();
                case "33": //BarometricPressure
                    return ParseString(str, 1).ToString();
                case "45": //Relative throttle position
                    return (ParseString(str, 1) * 100 / 255).ToString();
                case "46": //OutsideTemperature
                    return (ParseString(str, 1) - 40).ToString();
                case "5E": //EngineFuelRate
                    result = (ParseString(str, 2) / 20);
                    if (result < 0 || result > 3212)
                        result = -255;
                    return result.ToString();
            }
            return "ERROR";
        }

        public static string ParseVINMsg(string result) //VIN
        {
            try
            {
                if (result.Contains("STOPPED"))
                    return result;
                if (result.Contains("NO DATA") || result.Contains("ERROR"))
                    return result;
                string temp = result.Replace("\r\n", "");
                int index = temp.IndexOf("0: ");
                if (index < 0)
                    return ParseLongVIN(result);
                temp = temp.Substring(index);
                temp = temp.Replace("0: ", "");
                temp = temp.Replace("1: ", "");
                temp = temp.Replace("2: ", "");
                temp = temp.Trim();
                if (temp.Length > 59)
                    temp = temp.Substring(0, 59);
                var items = temp.Split(' ');
                string ret = "";
                foreach (var s in items)
                {
                    ret += (char)int.Parse(s, NumberStyles.HexNumber);
                }
                if (ret.Length > 17)
                    ret = ret.Substring(ret.Length - 17);

                //mask last 7 digits
                ret = ret.Substring(0, 10);
                ret += "0000000";
                return ret;
            }
            catch (Exception exp)
            {
                return exp.Message;
            }
        }

        public static string ParseLongVIN(string result) //VIN
        {
            if (result.Contains("STOPPED"))
                return result;
            if (result.Contains("NO DATA") || result.Contains("ERROR"))
                return result;
            var items = result.Replace("\r\n", "").Split(' ');
            if (items.Length < 36)
                return "ERROR";
            if (items[0].Trim() != "49")
                return "ERROR";
            string ret = "";
            int tint;
            char tchar;
            switch (items[1])
            {
                case "02": //VIN
                    tint = int.Parse(items[6], NumberStyles.HexNumber);
                    tchar = (char)tint;
                    ret += tchar.ToString();
                    for (int i = 10; i < 14; i++)
                    {
                        tint = int.Parse(items[i], NumberStyles.HexNumber);
                        tchar = (char)tint;
                        ret += tchar.ToString();
                    }
                    for (int i = 17; i < 21; i++)
                    {
                        tint = int.Parse(items[i], NumberStyles.HexNumber);
                        tchar = (char)tint;
                        ret += tchar.ToString();
                    }
                    for (int i = 24; i < 28; i++)
                    {
                        tint = int.Parse(items[i], NumberStyles.HexNumber);
                        tchar = (char)tint;
                        ret += tchar.ToString();
                    }
                    for (int i = 31; i < 35; i++)
                    {
                        tint = int.Parse(items[i], NumberStyles.HexNumber);
                        tchar = (char)tint;
                        ret += tchar.ToString();
                    }
                    //mask last 7 digits
                    ret = ret.Substring(0, 10);
                    ret += "0000000";
                    return ret;
            }
            return "ERROR";
        }

        public static Dictionary<string, string> GetPIDs()
        {
            //return PIDs we want to collect
            //the structure is <cmd, key>
            var ret = new Dictionary<string, string>
            {
                {"0110", "fr"},
                {"0104", "el"},
                {"0106", "stfb"},
                {"0107", "ltfb"},
                {"010C", "rpm"},
                {"010D", "spd"},
                {"0111", "tp"},
                {"011F", "rt"},
                {"0121", "dis"},
                {"0145", "rtp"},
                {"0146", "ot"},
                {"015E", "efr"}
            };
            //EngineLoad 
            //ShortTermFuelBank1 
            //LongTermFuelBank1 
            //EngineRPM 
            //Speed 
            //MAFFlowRate
            //ThrottlePosition 
            //Runtime 
            //DistancewithMIL 
            //RelativeThrottlePosition 
            //OutsideTemperature
            //EngineFuelRate
            return ret;
        }

        public static string GetEmulatorValue(string cmd)
        {
            var r = new Random();
            switch (cmd)
            {
                case "0104":
                    return r.Next(0, 100).ToString();
                case "0106":
                    return (r.Next(0, 200) - 100).ToString();
                case "0107":
                    return (r.Next(0, 200) - 100).ToString();
                case "010C":
                    return r.Next(0, 2500).ToString();
                case "010D":
                    return r.Next(5, 70).ToString();
                case "0110":
                    return r.Next(0, 50).ToString();
                case "0111":
                    return r.Next(0, 100).ToString();
                case "011F":
                    _runtime = _runtime + 2;
                    return _runtime.ToString();
                case "0121":
                    _distancewithMl = _distancewithMl + 0.1;
                    return ((int)_distancewithMl).ToString();
                case "0145":
                    return r.Next(0, 100).ToString();
                case "0146":
                    return SimulateTemperatureValue(r);
                case "015E":
                    return SimulateEngineFuelRateValue(r);
            }
            return "0";
        }

        static string SimulateTemperatureValue(Random r)
        {
            if (_outsideTemperature == "0")
            {
                _outsideTemperature = r.Next(0, 38).ToString();
            }
            else
            {
                var temperature = Double.Parse(_outsideTemperature);

                // Returns variance value between .01 and .05
                var variance = r.NextDouble() * (0.04) + .01;

                var varyTemperatureDirection = r.Next(0, 2);
                if (varyTemperatureDirection == 0)
                    temperature += variance;
                else
                    temperature -= variance;

                _outsideTemperature = temperature.ToString();
            }

            return _outsideTemperature;
        }

        static string SimulateEngineFuelRateValue(Random r)
        {
            var variance = r.NextDouble() * 6 + 8;
            return variance.ToString();
        }
    }
}
