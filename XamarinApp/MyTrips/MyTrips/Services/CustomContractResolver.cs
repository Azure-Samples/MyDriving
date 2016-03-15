using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyTrips.Services
{
    public class CustomContractResolver : DefaultContractResolver
    {
        private Dictionary<string, string> PropertyMappings { get; set; }

        private List<string> IgnoreProperties { get; set; }

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

            this.IgnoreProperties = new List<string>();
            this.IgnoreProperties.Add("HasOBDData");
        }

        protected override string ResolvePropertyName(string propertyName)
        {
            string resolvedName = null;
            var resolved = this.PropertyMappings.TryGetValue(propertyName, out resolvedName);
            return (resolved) ? resolvedName : base.ResolvePropertyName(propertyName);
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (this.IgnoreProperties.Contains(property.PropertyName))
            {
                property.ShouldSerialize = p => { return false; };
            }

            return property;
        }
    }
}
