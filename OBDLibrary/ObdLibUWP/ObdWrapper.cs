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
        const int Interval = 200;
        private StreamSocket _socket = null;
        private RfcommDeviceService _service = null;
        DataReader dataReaderObject = null;
        DataWriter dataWriterObject = null;
        string lastResponse;
        bool _connected = true;
        Dictionary<string, string> _data = null;
        bool _running = true;

        public async void Init()
        {
            //initialize _data
            this._data = new Dictionary<string, string>();
            this._data.Add("spd", "");  //Speed
            this._data.Add("bp", "");   //BarometricPressure
            this._data.Add("rpm", "");  //RPM
            this._data.Add("ot", "");   //OutsideTemperature
            this._data.Add("it", "");   //InsideTemperature
            this._data.Add("efr", "");   //EngineFuelRate

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
                return;
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
                    //    Read();
                    //}
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Overall Connect: " + ex.Message);
                _socket.Dispose();
                _socket = null;
            }
        }

        private async void PollObd()
        {
            while(this._running)
            {
                string s;
                s = await GetSpeed();
                if (s != "ERROR")
                    _data["spd"] = s;
                await Task.Delay(Interval);
                s = await GetBarometricPressure();
                if (s != "ERROR")
                    _data["bp"] = s;
                await Task.Delay(Interval);
                s = await GetRPM();
                if (s != "ERROR")
                    _data["rpm"] = s;
                await Task.Delay(Interval);
                s = await GetOutsideTemperature();
                if (s != "ERROR")
                    _data["ot"] = s;
                await Task.Delay(Interval);
                s = await GetInsideTemperature();
                if (s != "ERROR")
                    _data["it"] = s;
                await Task.Delay(Interval);
                s = await GetEngineFuelRate();
                if (s != "ERROR")
                    _data["efr"] = s;
                await Task.Delay(Interval);
            }
        }

        private string ParseObdMsg(string result)
        {
            if (result.Contains("STOPPED"))
                return "STOPPED";
            if (result.Contains("NO DATA") || result.Contains("ERROR"))
                return "ERROR";
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

        private async Task<string> ReadAsync()
        {
            string ret = await ReadAsyncRaw();
            string ret1 = ret.Replace("\r", "").Replace("\n", "");
            if ( ret1 == "" || ret1 == ">")
                ret = await ReadAsyncRaw();
            while (!ret.ToLower().Contains("\r"))
            {
                string tmp = await ReadAsyncRaw();
                ret = ret + tmp;
            }
            if (!ret.ToLower().Contains("searching"))
                return ret;
            ret = await ReadAsyncRaw();
            while (!ret.ToLower().Contains("\r"))
            {
                string tmp = await ReadAsyncRaw();
                ret = ret + tmp;
            }
            return ret;
        }

        public async Task<string> GetSpeed()
        {
            string result;
            result = await SendAndReceive("010D\r");
            //if(result == "STOPPED")
            //    result = await SendAndReceive("010D\r");
            return ParseObdMsg(result);
        }

        public async Task<string> GetOutsideTemperature()
        {
            string result;
            result = await SendAndReceive("0146\r");
            //if (result == "STOPPED")
            //    result = await SendAndReceive("0146\r");
            return ParseObdMsg(result);
        }

        public async Task<string> GetInsideTemperature()
        {
            string result;
            result = await SendAndReceive("010F\r");
            //if (result == "STOPPED")
            //    result = await SendAndReceive("010F\r");
            return ParseObdMsg(result);
        }

        public async Task<string> GetBarometricPressure()
        {
            string result;
            result = await SendAndReceive("0133\r");
            //if (result == "STOPPED")
            //    result = await SendAndReceive("0133\r");
            return ParseObdMsg(result);
        }

        public async Task<string> GetRPM()
        {
            string result;
            result = await SendAndReceive("010C\r");
            //if (result == "STOPPED")
            //    result = await SendAndReceive("010C\r");
            return ParseObdMsg(result);
        }

        public async Task<string> GetEngineFuelRate()
        {
            string result;
            result = await SendAndReceive("015E\r");
            //if (result == "STOPPED")
            //    result = await SendAndReceive("015E\r");
            return ParseObdMsg(result);
        }

        public Dictionary<string, string> Read()
        {
            return _data;
            //Dictionary<string, string> ret = new Dictionary<string, string>();
            //string s;
            //s = await GetSpeed();
            //if (s != "ERROR")
            //    ret.Add("0D", s);
            //s = await GetBarometricPressure();
            //if(s != "ERROR")
            //    ret.Add("33", s);
            //s = await GetRPM();
            //if (s != "ERROR")
            //    ret.Add("0C", s);
            //s = await GetOutsideTemperature();
            //if (s != "ERROR")
            //    ret.Add("46", s);
            //s = await GetInsideTemperature();
            //if (s != "ERROR")
            //    ret.Add("0F", s);
            //s = await GetEngineFuelRate();
            //if (s != "ERROR")
            //    ret.Add("5E", s);
            //return ret;
        }

        private async Task<string> SendAndReceive(string msg)
        {
            await WriteAsync(msg);
            string s = await ReadAsync();
            System.Diagnostics.Debug.WriteLine("Received: " + s);
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

        private async void Disconnect()
        {
            await this._socket.CancelIOAsync();
            _socket.Dispose();
            _socket = null;
            _running = false;
        }
    }
}
