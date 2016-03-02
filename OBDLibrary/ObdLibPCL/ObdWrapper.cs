using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Bluetooth;
using Android.Content;
using Java.Util;
using System.IO;
using System.Threading.Tasks;

namespace ObdLibPCL
{
    public class ObdWrapper
    {
        const int Interval = 500;
        const string DefValue = "";
        private BluetoothAdapter _bluetoothAdapter = null;
        private BluetoothDevice _bluetoothDevice = null;
        private BluetoothSocket _bluetoothSocket = null;
        private Stream _reader = null;
        private Stream _writer = null;
        private bool _connected = true;
        private static UUID SPP_UUID = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");
        Dictionary<string, string> _data = null;
        bool _running = true;
        private Object _lock = new Object();
        private bool _simulatormode;
        public async void Init(bool simulatormode = false)
        {
            //initialize _data
            this._data = new Dictionary<string, string>();
            this._data.Add("spd", "");  //Speed
            this._data.Add("bp", "");   //BarometricPressure
            this._data.Add("rpm", "");  //RPM
            this._data.Add("ot", "");   //OutsideTemperature
            this._data.Add("it", "");   //InsideTemperature
            this._data.Add("efr", "");  //EngineFuelRate
            this._data.Add("vin", "");  //VIN

            _simulatormode = simulatormode;
            if (simulatormode)
            {
                PollObd();

                ////these code is for testing.
                //while (true)
                //{
                //    await Task.Delay(2000);
                //    var dse = Read();
                //}

                return;
            }

            _bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            if (_bluetoothAdapter == null)
            {
                System.Diagnostics.Debug.WriteLine("Bluetooth is not available");
                return;
            }
            //if (!_bluetoothAdapter.IsEnabled)
            //{
            //    Intent enableIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
            //    StartActivityForResult(enableIntent, 2);
            //    // Otherwise, setup the chat session
            //}
            var ba = _bluetoothAdapter.BondedDevices;
            foreach (var bd in ba)
            {
                if (bd.Name.ToLower().Contains("obd"))
                    _bluetoothDevice = bd;
            }
            if (_bluetoothDevice == null)
            {
                return;
            }
            _bluetoothSocket = _bluetoothDevice.CreateRfcommSocketToServiceRecord(SPP_UUID);
            try
            {
                await _bluetoothSocket.ConnectAsync();
            }
            catch (Java.IO.IOException e)
            {
                // Close the socket
                try
                {
                    this._connected = false;
                    _bluetoothSocket.Close();
                }
                catch (Java.IO.IOException e2)
                {
                }

                return;
            }
            if (this._connected)
            {
                _reader = _bluetoothSocket.InputStream;
                _writer = _bluetoothSocket.OutputStream;

                string s;
                s = await SendAndReceive("ATZ\r");
                s = await SendAndReceive("ATE0\r");
                s = await SendAndReceive("ATL1\r");
                //s = await SendAndReceive("0100\r");
                s = await SendAndReceive("ATSP00\r");

                PollObd();

                //while(true)
                //{
                //    var dse = Read();
                //    await Task.Delay(2000);
                //}
            }
        }

        public Dictionary<string, string> Read()
        {
            if (!this._simulatormode && this._bluetoothSocket == null)
            {
                //if there is no connection
                return null;
            }
            var ret = new Dictionary<string, string>();
            lock (_lock)
            {
                foreach (var key in _data.Keys)
                {
                    ret.Add(key, _data[key]);
                }
                _data["spd"] = DefValue;  //Speed
                _data["bp"] = DefValue;   //BarometricPressure
                _data["rpm"] = DefValue;  //RPM
                _data["ot"] = DefValue;   //OutsideTemperature
                _data["it"] = DefValue;   //InsideTemperature
                _data["efr"] = DefValue;  //EngineFuelRate
            }
            return ret;
        }

        private async void PollObd()
        {
            try
            {
                string s;
                if (this._simulatormode)
                    s = "SIMULATOR12345678";
                else
                    s = await GetVIN();
                lock (_lock)
                {
                    _data["vin"] = s;
                }
                while (true)
                {
                    s = await GetSpeed();
                    if (s != "ERROR")
                        lock (_lock)
                        {
                            _data["spd"] = s;
                        }
                    if (!this._running)
                        break;
                    await Task.Delay(Interval);
                    s = await GetBarometricPressure();
                    if (s != "ERROR")
                        lock (_lock)
                        {
                            _data["bp"] = s;
                        }
                    if (!this._running)
                        break;
                    await Task.Delay(Interval);
                    s = await GetRPM();
                    if (s != "ERROR")
                        lock (_lock)
                        {
                            _data["rpm"] = s;
                        }
                    if (!this._running)
                        break;
                    await Task.Delay(Interval);
                    s = await GetOutsideTemperature();
                    if (s != "ERROR")
                        lock (_lock)
                        {
                            _data["ot"] = s;
                        }
                    if (!this._running)
                        break;
                    await Task.Delay(Interval);
                    s = await GetInsideTemperature();
                    if (s != "ERROR")
                        lock (_lock)
                        {
                            _data["it"] = s;
                        }
                    if (!this._running)
                        break;
                    await Task.Delay(Interval);
                    s = await GetEngineFuelRate();
                    if (s != "ERROR")
                        lock (_lock)
                        {
                            _data["efr"] = s;
                        }
                    if (!this._running)
                        break;
                    await Task.Delay(Interval);
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                _running = false;
                _bluetoothSocket.Dispose();
                _bluetoothSocket = null;
            }
        }

        public async Task<string> GetVIN()
        {
            string result;
            result = await SendAndReceive("0902\r");
            if (result.StartsWith("49"))
            {
                while (!result.Contains("49 02 05"))
                {
                    string tmp = await ReadAsync();
                    result += tmp;
                }
            }
            return ParseObd2Msg(result);
        }
        public async Task<string> GetSpeed()
        {
            if (_simulatormode)
            {
                var r = new System.Random();
                return r.Next().ToString();
            }
            string result;
            result = await SendAndReceive("010D\r");
            return ParseObdMsg(result);
        }
        public async Task<string> GetOutsideTemperature()
        {
            if (_simulatormode)
            {
                var r = new System.Random();
                return r.Next().ToString();
            }
            string result;
            result = await SendAndReceive("0146\r");
            return ParseObdMsg(result);
        }
        public async Task<string> GetInsideTemperature()
        {
            if (_simulatormode)
            {
                var r = new System.Random();
                return r.Next().ToString();
            }
            string result;
            result = await SendAndReceive("010F\r");
            return ParseObdMsg(result);
        }
        public async Task<string> GetBarometricPressure()
        {
            if (_simulatormode)
            {
                var r = new System.Random();
                return r.Next().ToString();
            }
            string result;
            result = await SendAndReceive("0133\r");
            return ParseObdMsg(result);
        }
        public async Task<string> GetRPM()
        {
            if (_simulatormode)
            {
                var r = new System.Random();
                return r.Next().ToString();
            }
            string result;
            result = await SendAndReceive("010C\r");
            return ParseObdMsg(result);
        }
        public async Task<string> GetEngineFuelRate()
        {
            if (_simulatormode)
            {
                var r = new System.Random();
                return r.Next().ToString();
            }
            string result;
            result = await SendAndReceive("015E\r");
            return ParseObdMsg(result);
        }
        private string ParseObdMsg(string result)
        {
            if (result.Contains("STOPPED"))
                return result;
            if (result.Contains("NO DATA") || result.Contains("ERROR"))
                return result;
            var items = result.Replace("\r", "").Replace("\n", "").Split(' ');
            if (items.Length < 3)
                return "ERROR";
            if (items[0].Trim() != "41")
                return "ERROR";
            int ret;
            switch (items[1])
            {
                case "0D":  //Speed
                    ret = int.Parse(items[2], System.Globalization.NumberStyles.HexNumber);
                    return ret.ToString();
                case "33":  //BarometricPressure
                    ret = int.Parse(items[2], System.Globalization.NumberStyles.HexNumber);
                    return ret.ToString();
                case "0C":  //RPM
                    ret = int.Parse(items[2] + items[3], System.Globalization.NumberStyles.HexNumber);
                    return (ret / 4).ToString();
                case "46":  //OutsideTemperature
                    ret = int.Parse(items[2], System.Globalization.NumberStyles.HexNumber);
                    return (ret - 40).ToString();
                case "0F":   //InsideTemperature
                    ret = int.Parse(items[2], System.Globalization.NumberStyles.HexNumber);
                    return (ret - 40).ToString();
                case "5E":  //EngineFuelRate
                    ret = int.Parse(items[2] + items[3], System.Globalization.NumberStyles.HexNumber);
                    return (ret / 20).ToString();
            }
            return "ERROR";
        }
        private string ParseObd2Msg(string result)  //VIN
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
        private async Task<string> SendAndReceive(string msg)
        {
            await WriteAsync(msg);
            string s = await ReadAsync();
            System.Diagnostics.Debug.WriteLine("Received: " + s);
            s = s.Replace("SEARCHING...\r\n", "");
            return s;
        }
        private async Task WriteAsync(string msg)
        {
            System.Diagnostics.Debug.WriteLine(msg);
            byte[] buffer = GetBytes(msg);
            await _writer.WriteAsync(buffer, 0, buffer.Length);
        }
        private byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
        private async Task<string> ReadAsync()
        {
            string ret = await ReadAsyncRaw();
            while (!ret.Trim().EndsWith("\r\n") && !ret.Trim().EndsWith("\r\r>") && !ret.Trim().EndsWith("\r\n>"))
            {
                string tmp = await ReadAsyncRaw();
                ret = ret + tmp;
            }
            return ret;
        }
        private async Task<string> ReadAsyncRaw()
        {
            byte[] buffer = new byte[1024];
            int bytes;
            bytes = await _reader.ReadAsync(buffer, 0, buffer.Length);
            var s1 = new Java.Lang.String(buffer, 0, bytes);
            var s = s1.ToString();
            System.Diagnostics.Debug.WriteLine(s);
            return s;
        }
        private void Disconnect()
        {
            _running = false;
            _bluetoothSocket.Dispose();
            _bluetoothSocket.Close();
            _bluetoothSocket = null;
        }
    }
}
