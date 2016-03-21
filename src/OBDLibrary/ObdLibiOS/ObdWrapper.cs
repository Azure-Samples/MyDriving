// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
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
        private readonly Object _lock = new Object();
        private bool connected = true;
        private Dictionary<string, string> data;
        private IPAddress ipAddress;
        private IPEndPoint ipEndPoint;
        private Dictionary<string, string> piDs;
        private int port;
        private bool running = true;
        private bool simulatormode;
        private Socket socket;
        private Stream stream;

        public async Task<bool> Init(bool simulatormode = false)
        {
            running = true;
            //initialize _data
            data = new Dictionary<string, string> {{"vin", DefValue}};
            //VIN
            piDs = ObdShare.ObdUtil.GetPIDs();
            foreach (var v in piDs.Values)
            {
                data.Add(v, DefValue);
            }

            this.simulatormode = simulatormode;
            if (simulatormode)
            {
                PollObd();
                
                return true;
            }
            bool isObdReaderAvailable = false;
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
                            isObdReaderAvailable = true;
                            break;
                        }
                    }
                }
            }
            if (!isObdReaderAvailable)
            {
                socket = null;
                running = false;
                connected = false;
                return false;
            }

            if (!ConnectSocket())
            {
                socket = null;
                running = false;
                connected = false;
                return false;
            }

            if (connected)
            {
                //initialize the device
                string s;
                s = await SendAndReceive("ATZ\r");
                s = await SendAndReceive("ATE0\r");
                s = await SendAndReceive("ATL1\r");
                s = await SendAndReceive("ATSP00\r");
                
                PollObd();
                
                return true;
            }
            else
                return false;
        }

        public Dictionary<string, string> Read()
        {
            if (!simulatormode && socket == null)
            {
                //if there is no connection
                return null;
            }
            var ret = new Dictionary<string, string>();
            lock (_lock)
            {
                foreach (var key in data.Keys)
                {
                    ret.Add(key, data[key]);
                }
                foreach (var v in piDs.Values)
                {
                    data[v] = DefValue;
                }
            }
            return ret;
        }

        private async void PollObd()
        {
            try
            {
                string s;
                if (simulatormode)
                    s = "SIMULATORIPHONE12";
                else
                    s = await GetVIN();
                lock (_lock)
                {
                    data["vin"] = s;
                }
                while (true)
                {
                    foreach (var cmd in piDs.Keys)
                    {
                        var key = piDs[cmd];
                        if (simulatormode)
                            s = ObdShare.ObdUtil.GetEmulatorValue(cmd);
                        else
                            s = await RunCmd(cmd);
                        if (s != "ERROR")
                            lock (_lock)
                            {
                                data[key] = s;
                            }
                        if (!running)
                            return;
                    }
                    await Task.Delay(Interval);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                running = false;
                if (stream != null)
                {
                    stream.Close();
                    stream = null;
                }
                if (socket != null)
                {
                    socket.Close();
                    socket = null;
                }
            }
        }

        private async Task<string> GetVIN()
        {
            var result = await SendAndReceive("0902\r");
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
            ipAddress = IPAddress.Parse("192.168.0.10"); //hard coded in obdlink MX
            port = 35000; //hard coded in obdlink MX
            ipEndPoint = new IPEndPoint(ipAddress, port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                socket.Connect(ipEndPoint);
                stream = new NetworkStream(socket);
                connected = true;
            }
            catch (Exception)
            {
                connected = false;
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
            catch (Exception ex)
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return "";
        }

        private async Task WriteAsync(string msg)
        {
            System.Diagnostics.Debug.WriteLine(msg);
            byte[] buffer = Encoding.ASCII.GetBytes(msg);
            await stream.WriteAsync(buffer, 0, buffer.Length);
            stream.Flush();
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
            var bytes = await stream.ReadAsync(buffer, 0, buffer.Length);
            var s = Encoding.ASCII.GetString(buffer, 0, bytes);
            System.Diagnostics.Debug.WriteLine(s);
            return s;
        }

        private async Task<string> RunCmd(string cmd)
        {
            var result = await SendAndReceive(cmd + "\r");
            return ObdShare.ObdUtil.ParseObd01Msg(result);
        }

        public async Task Disconnect()
        {
            running = false;
            if (stream != null)
            {
                stream.Dispose();
                stream.Close();
                stream = null;
            }
            if (socket != null)
            {
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
                socket.Close();
                socket = null;
            }
        }
    }
}