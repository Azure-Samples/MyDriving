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
        const string DefValue = "-255";
        private static readonly UUID SppUuid = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");
        private readonly Object _lock = new Object();
        private BluetoothAdapter _bluetoothAdapter;
        private BluetoothDevice _bluetoothDevice;
        private BluetoothSocket _bluetoothSocket;
        private bool _connected = true;
        private Dictionary<string, string> _data;
        private Dictionary<string, string> _PIDs;
        private Stream _reader;
        private bool _running = true;
        private bool _simulatormode;
        private Stream _writer;

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
                //PollObd();

                ////these code is for testing.
                //while (true)
                //{
                //    await Task.Delay(2000);
                //    var dse = Read();
                //}

                return true;
            }

            _bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            if (_bluetoothAdapter == null)
            {
                System.Diagnostics.Debug.WriteLine("Bluetooth is not available");
                return false;
            }
            //if (!_bluetoothAdapter.IsEnabled)
            //{
            //    Intent enableIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
            //    StartActivityForResult(enableIntent, 2);
            //    // Otherwise, setup the chat session
            //}
            try
            {
                var ba = _bluetoothAdapter.BondedDevices;
                foreach (var bd in ba)
                {
                    if (bd.Name.ToLower().Contains("obd"))
                        _bluetoothDevice = bd;
                }
                if (_bluetoothDevice == null)
                {
                    return false;
                }
                _bluetoothSocket = _bluetoothDevice.CreateRfcommSocketToServiceRecord(SppUuid);

                await _bluetoothSocket.ConnectAsync();
                _connected = true;
            }
            catch (Java.IO.IOException e)
            {
                // Close the socket
                try
                {
                    _connected = false;
                    _bluetoothSocket.Close();
                }
                catch (Java.IO.IOException e2)
                {
                }
                catch (Exception ex3)
                {
                }

                return false;
            }
            catch (Exception ex4)
            {
            }
            if (_connected)
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

                return true;
            }
            else
                return false;
        }

        public Dictionary<string, string> Read()
        {
            var ret = new Dictionary<string, string>();
            string s;
            if (_simulatormode)
            {
                s = "SIMULATORANDROID1";
                ret.Add("vin", s);
                foreach (var cmd in _PIDs.Keys)
                {
                    var key = _PIDs[cmd];
                    s = ObdShare.ObdUtil.GetEmulatorValue(cmd);
                    ret.Add(key, s);
                }
                return ret;
            }

            if (!_simulatormode && _bluetoothSocket == null)
            {
                //if there is no connection
                return null;
            }

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

        private async void PollObd()
        {
            try
            {
                string s;
                if (_simulatormode)
                    s = "SIMULATOR12345678";
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
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                _running = false;
                if (_reader != null)
                {
                    _reader.Close();
                    _reader = null;
                }
                if (_writer != null)
                {
                    _writer.Close();
                    _writer = null;
                }
                if (_bluetoothSocket != null)
                {
                    _bluetoothSocket.Close();
                    _bluetoothSocket = null;
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
            await _writer.WriteAsync(buffer, 0, buffer.Length);
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
            var bytes = await _reader.ReadAsync(buffer, 0, buffer.Length);
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
            _running = false;
            if (_reader != null)
            {
                _reader.Close();
                _reader = null;
            }
            if (_writer != null)
            {
                _writer.Close();
                _writer = null;
            }
            if (_bluetoothSocket != null)
            {
                try
                {
                    _bluetoothSocket.Close();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
                _bluetoothSocket = null;
            }
        }
    }
}