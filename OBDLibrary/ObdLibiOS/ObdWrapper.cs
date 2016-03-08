using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.IO;

namespace ObdLibiOS
{
    public class ObdWrapper
    {
        const uint BufSize = 1024;
        const int Interval = 500;
        const string DefValue = "";
        bool _connected = true;
        Dictionary<string, string> _data = null;
        private Object _lock = new Object();
        private bool _simulatormode;
        private IPAddress _ipAddress;
        private int _port;
        private IPEndPoint _ipEndPoint;
        private Socket _socket;
        private Stream _stream;
        bool _running = true;

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
                //PollObd();

                ////these code is for testing.
                //while (true)
                //{
                //    await Task.Delay(2000);
                //    var dse = Read();
                //}

                return true;
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
                    //s = GetSpeed();
                    //if (s != "ERROR")
                    //    lock (_lock)
                    //    {
                    //        _data["spd"] = s;
                    //    }
                    //if (!this._running)
                    //    break;
                    //System.Threading.Thread.Sleep(Interval);
                    //s = GetBarometricPressure();
                    //if (s != "ERROR")
                    //    lock (_lock)
                    //    {
                    //        _data["bp"] = s;
                    //    }
                    //if (!this._running)
                    //    break;
                    //System.Threading.Thread.Sleep(Interval);
                    s = await GetRPM();
                    if (s != "ERROR")
                        lock (_lock)
                        {
                            _data["rpm"] = s;
                        }
                    if (!this._running)
                        break;
                    await Task.Delay(Interval);
                    //s = GetOutsideTemperature();
                    //if (s != "ERROR")
                    //    lock (_lock)
                    //    {
                    //        _data["ot"] = s;
                    //    }
                    //if (!this._running)
                    //    break;
                    //System.Threading.Thread.Sleep(Interval);
                    //s = GetInsideTemperature();
                    //if (s != "ERROR")
                    //    lock (_lock)
                    //    {
                    //        _data["it"] = s;
                    //    }
                    //if (!this._running)
                    //    break;
                    //System.Threading.Thread.Sleep(Interval);
                    //s = GetEngineFuelRate();
                    //if (s != "ERROR")
                    //    lock (_lock)
                    //    {
                    //        _data["efr"] = s;
                    //    }
                    //if (!this._running)
                    //    break;
                    //System.Threading.Thread.Sleep(Interval);
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                _running = false;
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                _socket = null;
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
            return ObdShare.ObdUtil.ParseObd09Msg(result);
        }

        private async Task<string> GetRPM()
        {
            if (_simulatormode)
            {
                var r = new System.Random();
                return r.Next(0, 16383).ToString();
            }
            string result;
            result = await SendAndReceive("010C\r");
            return ObdShare.ObdUtil.ParseObd01Msg(result);
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
        //private void Write(string msg)
        //{
        //    System.Diagnostics.Debug.WriteLine(msg);
        //    byte[] buffer = Encoding.ASCII.GetBytes(msg);
        //    _socket.Send(buffer);
        //}
        private async Task WriteAsync(string msg)
        {
            System.Diagnostics.Debug.WriteLine(msg);
            byte[] buffer = Encoding.ASCII.GetBytes(msg);
            await _stream.WriteAsync(buffer, 0, buffer.Length);
            _stream.Flush();
        }
        //private string Receive()
        //{
        //    string ret = ReceiveRaw();
        //    while (!ret.Trim().EndsWith("\r\n") && !ret.Trim().EndsWith("\r\r>") && !ret.Trim().EndsWith("\r\n>"))
        //    {
        //        string tmp = ReceiveRaw();
        //        ret = ret + tmp;
        //    }
        //    return ret;
        //}
        private async Task<string> ReceiveAsync()
        {
            string ret = await ReceiveAsyncRaw();
            while (!ret.Trim().EndsWith("\r\n") && !ret.Trim().EndsWith("\r\r>") && !ret.Trim().EndsWith("\r\n>"))
            {
                string tmp = await ReceiveAsyncRaw();
                ret = ret + tmp;
            }
            return ret;
        }
        //private string ReceiveRaw()
        //{
        //    byte[] buffer = new byte[BufSize];
        //    int bytes;
        //    bytes = _socket.Receive(buffer);
        //    var s = Encoding.ASCII.GetString(buffer, 0, bytes);
        //    System.Diagnostics.Debug.WriteLine(s);
        //    return s;
        //}
        private async Task<string> ReceiveAsyncRaw()
        {
            byte[] buffer = new byte[BufSize];
            int bytes;
            bytes = await _stream.ReadAsync(buffer, 0, buffer.Length);
            var s = Encoding.ASCII.GetString(buffer, 0, bytes);
            System.Diagnostics.Debug.WriteLine(s);
            return s;
        }
        public void Disconnect()
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
