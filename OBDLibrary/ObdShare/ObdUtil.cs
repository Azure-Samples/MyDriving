using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace ObdShare
{
    public class ObdUtil
    {
        private static int ParseString(string str, int bytes)
        {
            return int.Parse(str.Substring(4, bytes * 2), NumberStyles.HexNumber);
        }
        public static string ParseObd01Msg(string input)
        {
            string str = input.Replace("\r", "").Replace("\n", "").Replace(" ", "").Replace(">", "").Trim();
            if (!str.StartsWith("41") || str.Length < 6)
                return str;
            string pid = str.Substring(2, 2);
            //int result;
            //var ret = int.TryParse(str.Substring(4), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out result);
            //if (!ret)
            //    return "ERROR";
            switch (pid)
            {
                case "04":  //EngineLoad
                    return (ParseString(str, 1) * 100 / 255).ToString();
                case "0C":  //RPM
                    return (ParseString(str, 2) / 4).ToString();
                case "0D":  //Speed
                    return ParseString(str, 1).ToString();
                case "10":  //MAF air flow rate
                    return (ParseString(str, 2) / 100).ToString();
                case "11":  //Throttle position                         
                    return (ParseString(str, 1) * 100 / 255).ToString();
                case "2C":  //Commanded EGR
                    return (ParseString(str, 1) * 100 / 255).ToString();
                case "2D":  //EGR Error
                    return ((ParseString(str, 1) - 128) * 100 / 128).ToString();
                case "45":  //Relative throttle position
                    return (ParseString(str, 1) * 100 / 255).ToString();                
                case "33":  //BarometricPressure
                    return ParseString(str, 1).ToString();
                case "46":  //OutsideTemperature
                    return (ParseString(str, 1) - 40).ToString();
                case "0F":   //InsideTemperature
                    return (ParseString(str, 1) - 40).ToString();
                case "5E":  //EngineFuelRate
                    return (ParseString(str, 2) / 20).ToString();
            }
            return "ERROR";
        }
        public static string ParseVINMsg(string result)  //VIN
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
                return ret;
            }
            catch (System.Exception exp)
            {
                return exp.Message;
            }
        }
        public static string ParseLongVIN(string result)  //VIN
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
                case "02":  //VIN
                    tint = int.Parse(items[6], System.Globalization.NumberStyles.HexNumber);
                    tchar = (char)tint;
                    ret += tchar.ToString();
                    for (int i = 10; i < 14; i++)
                    {
                        tint = int.Parse(items[i], System.Globalization.NumberStyles.HexNumber);
                        tchar = (char)tint;
                        ret += tchar.ToString();
                    }
                    for (int i = 17; i < 21; i++)
                    {
                        tint = int.Parse(items[i], System.Globalization.NumberStyles.HexNumber);
                        tchar = (char)tint;
                        ret += tchar.ToString();
                    }
                    for (int i = 24; i < 28; i++)
                    {
                        tint = int.Parse(items[i], System.Globalization.NumberStyles.HexNumber);
                        tchar = (char)tint;
                        ret += tchar.ToString();
                    }
                    for (int i = 31; i < 35; i++)
                    {
                        tint = int.Parse(items[i], System.Globalization.NumberStyles.HexNumber);
                        tchar = (char)tint;
                        ret += tchar.ToString();
                    }
                    return ret;
            }
            return "ERROR";
        }
    }
}
