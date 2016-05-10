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
        private bool connected = true;
        private Dictionary<string, string> data;
        private DataReader dataReaderObject;
        private DataWriter dataWriterObject;
        private Dictionary<string, string> piDs;
        private bool running = true;
        private RfcommDeviceService service;
        private bool simulatormode;
        private StreamSocket socket;

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
                service = await RfcommDeviceService.FromIdAsync(device.Id);

                // Disposing the socket with close it and release all resources associated with the socket
                socket?.Dispose();

                socket = new StreamSocket();
                try
                {
                    // Note: If either parameter is null or empty, the call will throw an exception
                    await socket.ConnectAsync(service.ConnectionHostName, service.ConnectionServiceName);
                    connected = true;
                }
                catch (Exception ex)
                {
                    connected = false;
                    System.Diagnostics.Debug.WriteLine("Connect:" + ex.Message);
                }
                // If the connection was successful, the RemoteAddress field will be populated
                if (connected)
                {
                    string msg = String.Format("Connected to {0}!", socket.Information.RemoteAddress.DisplayName);
                    System.Diagnostics.Debug.WriteLine(msg);

                    dataWriterObject = new DataWriter(socket.OutputStream);
                    dataReaderObject = new DataReader(socket.InputStream);

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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Overall Connect: " + ex.Message);
                if (dataReaderObject != null)
                {
                    dataReaderObject.Dispose();
                    dataReaderObject = null;
                }
                if (dataWriterObject != null)
                {
                    dataWriterObject.Dispose();
                    dataWriterObject = null;
                }
                if (socket != null)
                {
                    socket.Dispose();
                    socket = null;
                }
                return false;
            }
        }

        private async void PollObd()
        {
            try
            {
                string s;
                if (simulatormode)
                    s = "SIMULATORWINPHONE";
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
                if (dataReaderObject != null)
                {
                    dataReaderObject.Dispose();
                    dataReaderObject = null;
                }
                if (dataWriterObject != null)
                {
                    dataWriterObject.Dispose();
                    dataWriterObject = null;
                }
                if (socket != null)
                {
                    socket.Dispose();
                    socket = null;
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
            if (simulatormode)
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
                if (socket.OutputStream != null)
                {
                    //Launch the WriteAsync task to perform the write
                    await WriteAsync(msg);
                }
            }
            catch (Exception ex)
            {
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
                    string statusText = msg + ", ";
                    statusText += bytesWritten.ToString();
                    statusText += " bytes written successfully!";
                    System.Diagnostics.Debug.WriteLine(statusText);
                }
            }
        }

        private async Task<string> ReadAsyncRaw()
        {
            uint readBufferLength = 1024;

            // Set InputStreamOptions to complete the asynchronous read operation when one or more bytes is available
            dataReaderObject.InputStreamOptions = InputStreamOptions.Partial;

            // Create a task object to wait for data on the serialPort.InputStream
            var loadAsyncTask = dataReaderObject.LoadAsync(readBufferLength).AsTask();

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

        private async Task<string> RunCmd(string cmd)
        {
            var result = await SendAndReceive(cmd + "\r");
            return ObdShare.ObdUtil.ParseObd01Msg(result);
        }

        public async Task Disconnect()
        {
            running = false;
            if (dataReaderObject != null)
            {
                dataReaderObject.Dispose();
                dataReaderObject = null;
            }
            if (dataWriterObject != null)
            {
                dataWriterObject.Dispose();
                dataWriterObject = null;
            }
            if (socket != null)
            {
                try
                {
                    await socket.CancelIOAsync();
                    socket.Dispose();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
                socket = null;
            }
        }
    }
}