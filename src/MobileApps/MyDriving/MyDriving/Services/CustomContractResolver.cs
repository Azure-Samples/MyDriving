// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Reflection;

namespace MyDriving.Services
{
    public class CustomContractResolver : DefaultContractResolver
    {
        public CustomContractResolver()
        {
            PropertyMappings = new Dictionary<string, string>
            {
                ["Longitude"] = "Lon",
                ["Latitude"] = "Lat",
                ["ShortTermFuelBank"] = "ShortTermFuelBank1",
                ["LongTermFuelBank"] = "LongTermFuelBank1",
                ["MassFlowRate"] = "MAFFlowRate",
                ["RPM"] = "EngineRPM",
                ["Id"] = "TripPointId",
                ["DistanceWithMalfunctionLight"] = "DistancewithMIL",
                ["HasSimulatedOBDData"] = "IsSimulated",
            };

            IgnoreProperties = new List<string> {"HasOBDData"};
        }

        private Dictionary<string, string> PropertyMappings { get; }

        private List<string> IgnoreProperties { get; }

        protected override string ResolvePropertyName(string propertyName)
        {
            string resolvedName;
            var resolved = PropertyMappings.TryGetValue(propertyName, out resolvedName);
            return resolved ? resolvedName : base.ResolvePropertyName(propertyName);
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (IgnoreProperties.Contains(property.PropertyName))
            {
                property.ShouldSerialize = p => false;
            }

            return property;
        }
    }
}