// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using Android.Bluetooth;
using Java.Util;
using System.IO;
using System.Threading.Tasks;

namespace ObdLibAndroid
{
    public class ObdWrapper
    {
        const int Interval = 100;
        const string DefValue = "-255";
        private static readonly UUID SppUuid = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");
        private readonly Object _lock = new Object();
        private BluetoothAdapter bluetoothAdapter;
        private BluetoothDevice bluetoothDevice;
        private BluetoothSocket bluetoothSocket;
        private bool connected = true;
        private Dictionary<string, string> data;
        private Dictionary<string, string> piDs;
        private Stream reader;
        private bool running = true;
        private bool simulatormode;
        private Stream writer;

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
                return true;
            }

            bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            if (bluetoothAdapter == null)
            {
                System.Diagnostics.Debug.WriteLine("Bluetooth is not available");
                return false;
            }
            try
            {
                var ba = bluetoothAdapter.BondedDevices;
                foreach (var bd in ba)
                {
                    if (bd.Name.ToLower().Contains("obd"))
                        bluetoothDevice = bd;
                }
                if (bluetoothDevice == null)
                {
                    return false;
                }
                bluetoothSocket = bluetoothDevice.CreateRfcommSocketToServiceRecord(SppUuid);

                await bluetoothSocket.ConnectAsync();
                connected = true;
            }
            catch (Java.IO.IOException)
            {
                // Close the socket
                try
                {
                    connected = false;
                    bluetoothSocket.Close();
                }
                catch (Java.IO.IOException)
                {
                }
                catch (Exception)
                {
                }

                return false;
            }
            catch (Exception)
            {
            }
            if (connected)
            {
                reader = bluetoothSocket.InputStream;
                writer = bluetoothSocket.OutputStream;

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
            var ret = new Dictionary<string, string>();
            string s;
            if (simulatormode)
            {
                s = "SIMULATORANDROID1";
                ret.Add("vin", s);
                foreach (var cmd in piDs.Keys)
                {
                    var key = piDs[cmd];
                    s = ObdShare.ObdUtil.GetEmulatorValue(cmd);
                    ret.Add(key, s);
                }
                return ret;
            }

            if (!simulatormode && bluetoothSocket == null)
            {
                //if there is no connection
                return null;
            }

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
                    s = "SIMULATOR12345678";
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
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
                if (writer != null)
                {
                    writer.Close();
                    writer = null;
                }
                if (bluetoothSocket != null)
                {
                    bluetoothSocket.Close();
                    bluetoothSocket = null;
                }
            }
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
            await writer.WriteAsync(buffer, 0, buffer.Length);
        }

        private byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length*sizeof (char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
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

        private async Task<string> ReadAsyncRaw()
        {
            byte[] buffer = new byte[1024];
            var bytes = await reader.ReadAsync(buffer, 0, buffer.Length);
            var s1 = new Java.Lang.String(buffer, 0, bytes);
            var s = s1.ToString();
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
            if (reader != null)
            {
                reader.Close();
                reader = null;
            }
            if (writer != null)
            {
                writer.Close();
                writer = null;
            }
            if (bluetoothSocket != null)
            {
                try
                {
                    bluetoothSocket.Close();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
                bluetoothSocket = null;
            }
        }
    }
}