// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyDriving.Interfaces;
#if WINDOWS_UWP
using ObdLibUWP;

#elif __ANDROID__
using ObdLibAndroid;

#elif __IOS__
using ObdLibiOS;
#endif

namespace MyDriving.Shared
{
    public class OBDDevice : IOBDDevice
    {
        readonly ObdWrapper _obdWrapper = new ObdWrapper();

        public async Task Disconnect()
        {
            await _obdWrapper.Disconnect();
        }

        public async Task<bool> Initialize(bool simulatorMode = false)
        {
            IsSimulated = simulatorMode;
            return await _obdWrapper.Init(simulatorMode);
        }

        public Dictionary<String, String> ReadData()
        {
            return _obdWrapper.Read();
        }

        public bool IsSimulated { get; private set; }
    }
}