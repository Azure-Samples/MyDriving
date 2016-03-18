using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.IO;
using System.Net.NetworkInformation;

namespace ObdLibiOS
{
    public class ObdWrapper
    {
        const uint BufSize = 1024;
        const int Interval = 100;
        const string DefValue = "-255";
        private bool _connected = true;
        private Dictionary<string, string> _data = null;
        private Object _lock = new Object();
        private bool _simulatormode;
        private IPAddress _ipAddress;
        private int _port;
        private IPEndPoint _ipEndPoint;
        private Socket _socket;
        private Stream _stream;
        private bool _running = true;
        private Dictionary<string, string> _PIDs;

        public async Task<bool> Init(bool simulatormode = false)
        {
            this._running = true;
            //initialize _data
            this._data = new Dictionary<string, string>();
            this._data.Add("vin", DefValue);  //VIN
            _PIDs = ObdShare.ObdUtil.GetPIDs();
            foreach (var v in _PIDs.Values)
            {
                this._data.Add(v, DefValue);
            }

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
            bool IsObdReaderAvailable = false;
            foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 
                    || netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    foreach (var addrInfo in netInterface.GetIPProperties().UnicastAddresses)
                    {
                        var ipaddr = addrInfo.Address;
                        if (ipaddr.ToString().StartsWith("192.168.0"))
                        {
                            IsObdReaderAvailable = true;
                            break;
                        }
                    }
                }
            }
            if (!IsObdReaderAvailable)
            {
                this._socket = null;
                this._running = false;
                this._connected = false;
                return false;
            }

            if (!ConnectSocket())
            {
                this._socket = null;
                this._running = false;
                this._connected = false;
                return false;
            }
            
            if (this._connected)
            {
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
        public Dictionary<string, string> Read()
        {
            if (!this._simulatormode && this._socket == null)
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
                foreach (var v in _PIDs.Values)
                {
                    this._data[v] = DefValue;
                }
            }
            return ret;
        }
        private async void PollObd()
        {
            try
            {
                string s;
                if (this._simulatormode)
                    s = "SIMULATORIPHONE12";
                else
                    s = await GetVIN();
                lock (_lock)
                {
                    _data["vin"] = s;
                }
                while (true)
                {
                    foreach (var cmd in _PIDs.Keys)
                    {
                        var key = _PIDs[cmd];
                        if (_simulatormode)
                            s = ObdShare.ObdUtil.GetEmulatorValue(cmd);
                        else
                            s = await RunCmd(cmd);
                        if (s != "ERROR")
                            lock (_lock)
                            {
                                _data[key] = s;
                            }
                        if (!this._running)
                            return;
                        await Task.Delay(Interval);
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                _running = false;
                if (_stream != null)
                {
                    _stream.Dispose();
                    _stream.Close();
                    _stream = null;
                }
                if (_socket != null)
                {
                    _socket.Shutdown(SocketShutdown.Both);
                    _socket.Close();
                    _socket = null;
                }
            }
        }
        private async Task<string> GetVIN()
        {
            string result;
            result = await SendAndReceive("0902\r");
            if (result.StartsWith("49"))
            {
                while (!result.Contains("49 02 05"))
                {
                    string tmp = await ReceiveAsync();
                    result += tmp;
                }
            }
            return ObdShare.ObdUtil.ParseVINMsg(result);
        }
        private bool ConnectSocket()
        {
            //setup the connection via socket
            _ipAddress = IPAddress.Parse("192.168.0.10");   //hard coded in obdlink MX
            _port = 35000;  //hard coded in obdlink MX
            _ipEndPoint = new IPEndPoint(_ipAddress, _port);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        
            try
            {
                _socket.Connect(_ipEndPoint);
                _stream = new NetworkStream(_socket);
                this._connected = true;
            }
            catch (Exception ex)
            {
                this._connected = false;
                return false;
            }
            return true;
        }
        private async Task<string> SendAndReceive(string msg)
        {
            try
            {
                await WriteAsync(msg);
            }
            catch(System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            System.Threading.Thread.Sleep(100);
            try
            {
                string s = await ReceiveAsync();
                System.Diagnostics.Debug.WriteLine("Received: " + s);
                s = s.Replace("SEARCHING...\r\n", "");
                return s;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return "";
        }
        private async Task WriteAsync(string msg)
        {
            System.Diagnostics.Debug.WriteLine(msg);
            byte[] buffer = Encoding.ASCII.GetBytes(msg);
            await _stream.WriteAsync(buffer, 0, buffer.Length);
            _stream.Flush();
        }
        private async Task<string> ReceiveAsync()
        {
            string ret = await ReceiveAsyncRaw();
            while (!ret.Trim().EndsWith(">"))
            {
                string tmp = await ReceiveAsyncRaw();
                ret = ret + tmp;
            }
            return ret;
        }
        private async Task<string> ReceiveAsyncRaw()
        {
            byte[] buffer = new byte[BufSize];
            int bytes;
            bytes = await _stream.ReadAsync(buffer, 0, buffer.Length);
            var s = Encoding.ASCII.GetString(buffer, 0, bytes);
            System.Diagnostics.Debug.WriteLine(s);
            return s;
        }
        private async Task<string> RunCmd(string cmd)
        {
            string result;
            result = await SendAndReceive(cmd + "\r");
            return ObdShare.ObdUtil.ParseObd01Msg(result);
        }
        public async Task Disconnect()
        {
            _running = false;
            if(_stream != null)
            {
                _stream.Dispose();
                _stream.Close();
                _stream = null;
            }
            if (_socket != null)
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                _socket = null;
            }
        }
    }
}
