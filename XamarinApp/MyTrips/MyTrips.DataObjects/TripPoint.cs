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
        public string TripId { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the speed, in km/h
        /// </summary>
        /// <value>The speed.</value>
        public double Speed { get; set; }

        public DateTime RecordedTimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the sequence order number starting at 0
        /// </summary>
        /// <value>The sequence.</value>
        public int Sequence {get;set;}

        public double RPM { get; set;}

        public double ShortTermFuelBank { get; set; }

        public double LongTermFuelBank { get; set; }

        public double ThrottlePosition { get; set; }

        public double RelativeThrottlePosition { get; set; }

        public double Runtime { get; set; }

        public double DistanceWithMalfunctionLight { get; set; }

        public double EngineLoad { get; set; }

        public double MassFlowRate { get; set; }

        public double OutsideTemperature { get; set; }

        public double EngineFuelRate { get; set; }
        public bool IsSimulated { get; set; }

        public string VIN { get; set; }

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