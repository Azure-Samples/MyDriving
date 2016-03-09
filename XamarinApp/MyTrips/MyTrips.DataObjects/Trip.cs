using System;
using System.Collections.Generic;

#if !BACKEND
using System.Text;
using Humanizer;
using Newtonsoft.Json;
using MvvmHelpers;
using MyTrips.Utils;
#endif

namespace MyTrips.DataObjects
{
    /// <summary>
    /// Note that this is the structure that was discussed with Haishi - is subject to change.
    /// </summary>
    public class Trip : BaseDataObject
    {
        public Trip()
        {
            Points = new List<TripPoint>();
            Tips = new List<Tip>();
        }

        public string Name { get; set; }

        public string UserId { get; set; }

        public IList<TripPoint> Points { get; set; }

        public DateTime RecordedTimeStamp { get; set; }
        public DateTime EndTimeStamp { get; set; }

        public int Rating { get; set; }

        public bool IsComplete { get; set; }

        public double AverageSpeed { get; set; }

        public double FuelUsed { get; set; }

        public string MainPhotoUrl { get; set; }

        #if BACKEND
        public double Distance { get; set; } 
        #else
        double distance;
        /// <summary>
        /// Gets or sets the total distance in miles.
        /// </summary>
        /// <value>The total distance.</value>
        public double Distance
        {
            get { return distance; }
            set { SetProperty(ref distance, value); }
        }

        //Do not sync with backend, used localy only
        [JsonIgnore]
        public string TimeAgo => EndTimeStamp.Humanize();

        [JsonIgnore]
        public double DistanceConverted => (Settings.Current.MetricDistance ? (Distance * 1.60934) : Distance);

        [JsonIgnore]
        public string Units => (Settings.Current.MetricDistance ? "km" : "miles");

        [JsonIgnore]
        public string TotalDistance => DistanceConverted.ToString("f") + " "  + Units;

        [JsonIgnore]
        public string TotalDistanceNoUnits => DistanceConverted.ToString("f");

        [JsonIgnore]
        public string StartTimeDisplay => RecordedTimeStamp.ToLocalTime().ToString("t");

        [JsonIgnore]
        public string EndTimeDisplay => (Points?.Count).GetValueOrDefault() > 0 ? Points[Points.Count - 1].RecordedTimeStamp.ToLocalTime().ToString("t") : string.Empty;

        [JsonIgnore]
        public IList<Photo> Photos { get; set; }

        #endif
    }
}
