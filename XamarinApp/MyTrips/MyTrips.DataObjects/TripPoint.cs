using System;
using System.Collections.Generic;
#if !BACKEND
using Newtonsoft.Json;
using MyTrips.Utils;
#endif
namespace MyTrips.DataObjects
{
    public class TripPoint: BaseDataObject
    {
        [Newtonsoft.Json.JsonProperty("TripPointId")]
        public override string Id { get; set; }

        [Newtonsoft.Json.JsonProperty("Lat")]
        public double Latitude { get; set; }

        [Newtonsoft.Json.JsonProperty("Lon")]
        public double Longitude { get; set; }

        public double Speed { get; set; }

        public DateTime RecordedTimeStamp { get; set; }

        public int Sequence {get;set;}

        public double EngineRPM { get; set;}

        [Newtonsoft.Json.JsonProperty("ShortTermFuelBank1")]
        public double ShortTermFuelBank { get; set; }

        [Newtonsoft.Json.JsonProperty("LongTermFuelBank1")]
        public double LongTermFuelBank { get; set; }

        public double ThrottlePosition { get; set; }

        public double RelativeThrottlePosition { get; set; }

        public double Runtime { get; set; }

        [Newtonsoft.Json.JsonProperty("DistancewithMIL")]
        public double DistWithMalfunctionLight { get; set; }

        public double EngineLoad { get; set; }

        [Newtonsoft.Json.JsonProperty("MAFFlowRate")]
        public double FlowRate { get; set; }

        public double OutsideTemperature { get; set; }

        public double EngineFuelRate { get; set; }

        public string VIN { get; set; }

        [JsonIgnore]
        public bool HasOBDData { get; set; }

        #if !BACKEND
        [JsonIgnore]
        public string DisplayTemp
        {
            get 
            {
                if (!HasOBDData)
                    return "N/A";
                
                return  Settings.Current.MetricTemp ? (OutsideTemperature.ToString("N1") + "°C") :
                                                 (((OutsideTemperature * 1.8) + 32).ToString("N1") + "°F");
            }
        }
        #endif
    }
}