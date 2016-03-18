// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace ObdLibUWP
{
    public class ObdWrapper
    {
        const int Interval = 100;
        const string DefValue = "-255";
        private readonly Object _lock = new Object();
        private bool _connected = true;
        private Dictionary<string, string> _data;
        private DataReader _dataReaderObject;
        private DataWriter _dataWriterObject;
        private Dictionary<string, string> _PIDs;
        private bool _running = true;
        private RfcommDeviceService _service;
        private bool _simulatormode;
        private StreamSocket _socket;

        public async Task<bool> Init(bool simulatormode = false)
        {
            _running = true;
            //initialize _data
            _data = new Dictionary<string, string> {{"vin", DefValue}};
            //VIN
            _PIDs = ObdShare.ObdUtil.GetPIDs();
            foreach (var v in _PIDs.Values)
            {
                _data.Add(v, DefValue);
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

            DeviceInformationCollection deviceInfoCollection =
                await DeviceInformation.FindAllAsync(RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort));
            var numDevices = deviceInfoCollection.Count();
            DeviceInformation device = null;
            foreach (DeviceInformation info in deviceInfoCollection)
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

                // Disposing the socket with close it and release all resources associated with the socket
                _socket?.Dispose();

                _socket = new StreamSocket();
                try
                {
                    // Note: If either parameter is null or empty, the call will throw an exception
                    await _socket.ConnectAsync(_service.ConnectionHostName, _service.ConnectionServiceName);
                    _connected = true;
                }
                catch (Exception ex)
                {
                    _connected = false;
                    System.Diagnostics.Debug.WriteLine("Connect:" + ex.Message);
                }
                // If the connection was successful, the RemoteAddress field will be populated
                if (_connected)
                {
                    string msg = String.Format("Connected to {0}!", _socket.Information.RemoteAddress.DisplayName);
                    System.Diagnostics.Debug.WriteLine(msg);

                    _dataWriterObject = new DataWriter(_socket.OutputStream);
                    _dataReaderObject = new DataReader(_socket.InputStream);

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
                if (_dataReaderObject != null)
                {
                    _dataReaderObject.Dispose();
                    _dataReaderObject = null;
                }
                if (_dataWriterObject != null)
                {
                    _dataWriterObject.Dispose();
                    _dataWriterObject = null;
                }
                if (_socket != null)
                {
                    _socket.Dispose();
                    _socket = null;
                }
                return false;
            }
        }

        private async void PollObd()
        {
            try
            {
                string s;
                if (_simulatormode)
                    s = "SIMULATORWINPHONE";
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
                        if (!_running)
                            return;
                        await Task.Delay(Interval);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                _running = false;
                if (_dataReaderObject != null)
                {
                    _dataReaderObject.Dispose();
                    _dataReaderObject = null;
                }
                if (_dataWriterObject != null)
                {
                    _dataWriterObject.Dispose();
                    _dataWriterObject = null;
                }
                if (_socket != null)
                {
                    await _socket.CancelIOAsync();
                    _socket.Dispose();
                    _socket = null;
                }
            }
        }

        private async Task<string> ReadAsync()
        {
            string ret = await ReadAsyncRaw();
            while (!ret.Trim().EndsWith(">"))
            {
                string tmp = await ReadAsyncRaw();
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
            var result = await SendAndReceive("010D\r");
            return ObdShare.ObdUtil.ParseObd01Msg(result);
        }

        public async Task<string> GetVIN()
        {
            var result = await SendAndReceive("0902\r");
            if (result.StartsWith("49"))
            {
                while (!result.Contains("49 02 05"))
                {
                    string tmp = await ReadAsync();
                    result += tmp;
                }
            }
            return ObdShare.ObdUtil.ParseVINMsg(result);
        }

        public Dictionary<string, string> Read()
        {
            if (!_simulatormode && _socket == null)
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
                    _data[v] = DefValue;
                }
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
                if (_dataWriterObject != null)
                {
                    _dataWriterObject.DetachStream();
                    _dataWriterObject = null;
                }
            }
        }

        private async Task WriteAsync(string msg)
        {
            Task<UInt32> storeAsyncTask;

            if (msg.Length != 0)
            {
                // Load the text from the sendText input text box to the dataWriter object
                _dataWriterObject.WriteString(msg);

                // Launch an async task to complete the write operation
                storeAsyncTask = _dataWriterObject.StoreAsync().AsTask();

                UInt32 bytesWritten = await storeAsyncTask;
                if (bytesWritten > 0)
                {
                    string statusText = msg + ", ";
                    statusText += bytesWritten.ToString();
                    statusText += " bytes written successfully!";
                    System.Diagnostics.Debug.WriteLine(statusText);
                }
            }
        }

        private async Task<string> ReadAsyncRaw()
        {
            uint ReadBufferLength = 1024;

            // Set InputStreamOptions to complete the asynchronous read operation when one or more bytes is available
            _dataReaderObject.InputStreamOptions = InputStreamOptions.Partial;

            // Create a task object to wait for data on the serialPort.InputStream
            var loadAsyncTask = _dataReaderObject.LoadAsync(ReadBufferLength).AsTask();

            // Launch the task and wait
            UInt32 bytesRead = await loadAsyncTask;
            if (bytesRead > 0)
            {
                try
                {
                    string recvdtxt = _dataReaderObject.ReadString(bytesRead);
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

        private async Task<string> RunCmd(string cmd)
        {
            var result = await SendAndReceive(cmd + "\r");
            return ObdShare.ObdUtil.ParseObd01Msg(result);
        }

        public async Task Disconnect()
        {
            _running = false;
            if (_dataReaderObject != null)
            {
                _dataReaderObject.Dispose();
                _dataReaderObject = null;
            }
            if (_dataWriterObject != null)
            {
                _dataWriterObject.Dispose();
                _dataWriterObject = null;
            }
            if (_socket != null)
            {
                try
                {
                    await _socket.CancelIOAsync();
                    _socket.Dispose();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
                _socket = null;
            }
        }
    }
}