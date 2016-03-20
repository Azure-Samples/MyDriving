// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace VINParser
{
    public class VMIProvider
    {
        private readonly Dictionary<string, string> mMakeLookup;
        private readonly Dictionary<string, MakeMap> mMakeMaps = new Dictionary<string, MakeMap>();
        private readonly Dictionary<string, CarInfo> mWmiLookup;

        public VMIProvider()
        {
        }

        public VMIProvider(string vmiJson)
        {
            mWmiLookup = DeserializeJsonFile<Dictionary<string, CarInfo>>(vmiJson);

            FileInfo info = new FileInfo(vmiJson);
            var manufactureFile = Path.Combine(info.Directory.FullName, "manufactures.json");
            if (File.Exists(manufactureFile))
            {
                mMakeLookup = DeserializeJsonFile<Dictionary<string, string>>(manufactureFile);
                foreach (string key in mMakeLookup.Keys)
                {
                    mMakeMaps.Add(key,
                        DeserializeJsonFile<MakeMap>(Path.Combine(info.Directory.FullName, mMakeLookup[key])));
                }
            }
        }

        private T DeserializeJsonFile<T>(string file)
        {
            using (StreamReader reader = new StreamReader(File.OpenRead(file)))
            {
                using (JsonTextReader jReader = new JsonTextReader(reader))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    return serializer.Deserialize<T>(jReader);
                }
            }
        }

        public CarInfo GetCarInfo(string vin)
        {
            CarInfo ret = null;
            if (mWmiLookup != null)
            {
                string key = vin.Substring(0, 3);
                if (mWmiLookup.ContainsKey(key))
                    ret = mWmiLookup[key];
                else
                {
                    key = vin.Substring(0, 2);
                    if (mWmiLookup.ContainsKey(key))
                        ret = mWmiLookup[key];
                }
            }
            if (ret == null)
            {
                ret = new CarInfo
                {
                    CarType = CarType.Unknown,
                    Make = "Unknown",
                    Model = "Unknown"
                };
            }

            if (mMakeLookup != null)
            {
                MakeMap map = null;
                if (mMakeMaps.ContainsKey(vin.Substring(0, 3)))
                    map = mMakeMaps[vin.Substring(0, 3)];
                else if (mMakeLookup.ContainsKey(vin.Substring(0, 2)))
                    map = mMakeMaps[vin.Substring(0, 2)];
                if (map != null)
                {
                    ExpandCarInfo(ret, map, vin);
                }
            }

            if (string.IsNullOrEmpty(ret.Model))
                ret.Model = "Unknown";
            if (string.IsNullOrEmpty(ret.Make))
                ret.Make = "Unknown";
            return ret;
        }

        private void ExpandCarInfo(CarInfo info, MakeMap map, string vin)
        {
            for (int i = map.MaxLength; i >= 1; i--)
            {
                string part = vin.Substring(map.StartDigit - 1, i);
                if (map.Map.ContainsKey(part))
                {
                    if (map.Map[part].CarType != CarType.Unknown)
                        info.CarType = map.Map[part].CarType;
                    if (!string.IsNullOrEmpty(map.Map[part].Model))
                        info.Model = map.Map[part].Model;
                    return;
                }
            }
        }
    }
}