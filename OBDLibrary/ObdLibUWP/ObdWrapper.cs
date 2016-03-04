using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using System.Threading;

namespace ObdLibUWP
{
    public class ObdWrapper
    {
        const uint BufSize = 64;
        const int Interval = 500;
        const string DefValue = "";
        private StreamSocket _socket = null;
        private RfcommDeviceService _service = null;
        DataReader dataReaderObject = null;
        DataWriter dataWriterObject = null;
        string lastResponse;
        bool _connected = true;
        Dictionary<string, string> _data = null;
        bool _running = true;
        private Object _lock = new Object();
        private bool _simulatormode;

        public async Task<bool> Init(bool simulatormode = false)
        {
            //initialize _data
            this._data = new Dictionary<string, string>();
            this._data.Add("spd", DefValue);  //Speed
            this._data.Add("bp", DefValue);   //BarometricPressure
            this._data.Add("rpm", DefValue);  //RPM
            this._data.Add("ot", DefValue);   //OutsideTemperature
            this._data.Add("it", DefValue);   //InsideTemperature
            this._data.Add("efr", DefValue);  //EngineFuelRate
            this._data.Add("vin", DefValue);  //VIN

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

                return true;
            }
            
            DeviceInformationCollection DeviceInfoCollection = await DeviceInformation.FindAllAsync(RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort));
            var numDevices = DeviceInfoCollection.Count();
            DeviceInformation device = null;
            foreach (DeviceInformation info in DeviceInfoCollection)
            {
                if (info.Name.ToLower().Contains("obd"))
                {
                    device = info;
                }
            }
            if (device == null)
                return false;
            try
            {
                _service = await RfcommDeviceService.FromIdAsync(device.Id);

                if (_socket != null)
                {
                    // Disposing the socket with close it and release all resources associated with the socket
                    _socket.Dispose();
                }

                _socket = new StreamSocket();
                try { 
                    // Note: If either parameter is null or empty, the call will throw an exception
                    await _socket.ConnectAsync(_service.ConnectionHostName, _service.ConnectionServiceName);
                }
                catch (Exception ex)
                {
                        this._connected = false;
                        System.Diagnostics.Debug.WriteLine("Connect:" + ex.Message);
                }
                // If the connection was successful, the RemoteAddress field will be populated
                if (this._connected)
                {
                    string msg = String.Format("Connected to {0}!", _socket.Information.RemoteAddress.DisplayName);
                    System.Diagnostics.Debug.WriteLine(msg);

                    dataWriterObject = new DataWriter(_socket.OutputStream);
                    dataReaderObject = new DataReader(_socket.InputStream);

                    //initialize the device
                    string s;
                    s = await SendAndReceive("ATZ\r");
                    s = await SendAndReceive("ATE0\r");
                    s = await SendAndReceive("ATL1\r");
                    //s = await SendAndReceive("0100\r");
                    s = await SendAndReceive("ATSP00\r");

                    PollObd();

                    ////these code is for testing.
                    //while (true)
                    //{
                    //    await Task.Delay(2000);
                    //    var dse = Read();
                    //}

                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Overall Connect: " + ex.Message);
                _socket.Dispose();
                _socket = null;
                return false;
            }
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
                lock(_lock)
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
                await this._socket.CancelIOAsync();
                _socket.Dispose();
                _socket = null;
            }
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

        private async Task<string> ReadAsync()
        {
            string ret = await ReadAsyncRaw();
            //string ret1 = ret.Replace("\r", "").Replace("\n", "");
            //if ( ret1 == "" || ret1 == ">")
            //    ret = await ReadAsyncRaw();
            //while (!ret.ToLower().Contains("\r"))
            //{
            //    string tmp = await ReadAsyncRaw();
            //    ret = ret + tmp;
            //}
            //if (!ret.ToLower().Contains("searching"))
            //    return ret;
            //ret = await ReadAsyncRaw();
            //while (!ret.ToLower().Contains("\r"))
            //{
            //    string tmp = await ReadAsyncRaw();
            //    ret = ret + tmp;
            //}
            while (!ret.Trim().EndsWith("\r\n") && !ret.Trim().EndsWith("\r\r>") && !ret.Trim().EndsWith("\r\n>"))
            {
                string tmp =  await ReadAsyncRaw();
                ret = ret + tmp;
            }
            return ret;
        }

        public async Task<string> GetSpeed()
        {
            if (_simulatormode)
            {
                var r = new Random();
                return r.Next().ToString();
            }
            string result;
            result = await SendAndReceive("010D\r");
            //if(result == "STOPPED")
            //    result = await SendAndReceive("010D\r");
            return ParseObdMsg(result);
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
            //if(result == "STOPPED")
            //    result = await SendAndReceive("010D\r");
            return ParseObd2Msg(result);
        }

        public async Task<string> GetOutsideTemperature()
        {
            if (_simulatormode)
            {
                var r = new Random();
                return r.Next().ToString();
            }
            string result;
            result = await SendAndReceive("0146\r");
            //if (result == "STOPPED")
            //    result = await SendAndReceive("0146\r");
            return ParseObdMsg(result);
        }

        public async Task<string> GetInsideTemperature()
        {
            if (_simulatormode)
            {
                var r = new Random();
                return r.Next().ToString();
            }
            string result;
            result = await SendAndReceive("010F\r");
            //if (result == "STOPPED")
            //    result = await SendAndReceive("010F\r");
            return ParseObdMsg(result);
        }

        public async Task<string> GetBarometricPressure()
        {
            if (_simulatormode)
            {
                var r = new Random();
                return r.Next().ToString();
            }
            string result;
            result = await SendAndReceive("0133\r");
            //if (result == "STOPPED")
            //    result = await SendAndReceive("0133\r");
            return ParseObdMsg(result);
        }

        public async Task<string> GetRPM()
        {
            if (_simulatormode)
            {
                var r = new Random();
                return r.Next().ToString();
            }
            string result;
            result = await SendAndReceive("010C\r");
            //if (result == "STOPPED")
            //    result = await SendAndReceive("010C\r");
            return ParseObdMsg(result);
        }

        public async Task<string> GetEngineFuelRate()
        {
            if (_simulatormode)
            {
                var r = new Random();
                return r.Next().ToString();
            }
            string result;
            result = await SendAndReceive("015E\r");
            //if (result == "STOPPED")
            //    result = await SendAndReceive("015E\r");
            return ParseObdMsg(result);
        }

        public Dictionary<string, string> Read()
        {
            if(!this._simulatormode && this._socket == null)
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

        private async Task<string> SendAndReceive(string msg)
        {
            await WriteAsync(msg);
            string s = await ReadAsync();
            System.Diagnostics.Debug.WriteLine("Received: " + s);
            s = s.Replace("SEARCHING...\r\n", "");
            return s;
        }

        private async void Send(string msg)
        {
            try
            {
                if (_socket.OutputStream != null)
                {
                    //Launch the WriteAsync task to perform the write
                    await WriteAsync(msg);
                }
                else
                {
                    //status.Text = "Select a device and connect";
                }
            }
            catch (Exception ex)
            {
                //status.Text = "Send(): " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Send(): " + ex.Message);
            }
            finally
            {
                // Cleanup once complete
                if (dataWriterObject != null)
                {
                    dataWriterObject.DetachStream();
                    dataWriterObject = null;
                }
            }
        }

        private async Task WriteAsync(string msg)
        {
            Task<UInt32> storeAsyncTask;
            
            if (msg.Length != 0)
            {
                // Load the text from the sendText input text box to the dataWriter object
                dataWriterObject.WriteString(msg);

                // Launch an async task to complete the write operation
                storeAsyncTask = dataWriterObject.StoreAsync().AsTask();

                UInt32 bytesWritten = await storeAsyncTask;
                if (bytesWritten > 0)
                {
                    string status_Text = msg + ", ";
                    status_Text += bytesWritten.ToString();
                    status_Text += " bytes written successfully!";
                    System.Diagnostics.Debug.WriteLine(status_Text);
                    this.lastResponse = status_Text;
                }
            }
        }
        
        private async Task<string> ReadAsyncRaw()
        {
            Task<UInt32> loadAsyncTask;
            uint ReadBufferLength = 1024;
            
            // Set InputStreamOptions to complete the asynchronous read operation when one or more bytes is available
            dataReaderObject.InputStreamOptions = InputStreamOptions.Partial;

            // Create a task object to wait for data on the serialPort.InputStream
            loadAsyncTask = dataReaderObject.LoadAsync(ReadBufferLength).AsTask();

            // Launch the task and wait
            UInt32 bytesRead = await loadAsyncTask;
            if (bytesRead > 0)
            {
                try
                {
                    string recvdtxt = dataReaderObject.ReadString(bytesRead);
                    System.Diagnostics.Debug.WriteLine(recvdtxt);
                    return recvdtxt;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("ReadAsync: " + ex.Message);
                    return "";
                }
            }
            return "";
        }

        public async Task Disconnect()
        {
            _running = false;
            await this._socket.CancelIOAsync();
            _socket.Dispose();
            _socket = null;
        }
    }
}
