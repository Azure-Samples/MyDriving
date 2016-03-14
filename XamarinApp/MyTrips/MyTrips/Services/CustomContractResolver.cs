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
            this.PropertyMappings = new Dictionary<string, string>();
            this.PropertyMappings.Add("Longitude", "Lon");
            this.PropertyMappings.Add("Latitude", "Lat");
            this.PropertyMappings.Add("ShortTermFuelBank", "ShortTermFuelBank1");
            this.PropertyMappings.Add("LongTermFuelBank", "LongTermFuelBank1");
            this.PropertyMappings.Add("MassFlowRate", "MAFFlowRate");
            this.PropertyMappings.Add("RPM", "EngineRPM");
            this.PropertyMappings.Add("Id", "TripPointId");
            this.PropertyMappings.Add("DistanceWithMalfunctionLight", "DistancewithMIL");

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
