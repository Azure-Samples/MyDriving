﻿using System;
using System.Collections.Generic;
using System.Text;
using Humanizer;
using Newtonsoft.Json;

namespace MyTrips.DataObjects
{
    /// <summary>
    /// Note that this is the structure that was discussed with Haishi - is subject to change.
    /// </summary>
    public class Trip : BaseDataObject
    {
        public Trip()
        {
            this.Trail = new List<Trail>();
        }

        /// <summary>
        /// This is actually the name
        /// </summary>
        /// <value>The trip identifier.</value>
        public string TripId { get; set; }

        public string UserId { get; set; }

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

        public DateTime RecordedTimeStamp { get; set; }

        public int Rating { get; set; }

        //Do not sync with backend, used localy only
        [JsonIgnore]
        public string TimeAgo => RecordedTimeStamp.Humanize();

        [JsonIgnore]
        public string TotalDistance => Distance.ToString("F") + " miles";

        [JsonIgnore]
        public string TotalDistanceNoUnits => Distance.ToString("f");

        [JsonIgnore]
        public string StartTimeDisplay => RecordedTimeStamp.ToLocalTime().ToString("t");

        [JsonIgnore]
        public string EndTimeDisplay => (Trail?.Count).GetValueOrDefault() > 0 ? Trail[Trail.Count - 1].RecordedTimeStamp.ToLocalTime().ToString("t") : string.Empty;

        [JsonIgnore]
        public IList<Photo> Photos { get; set; }

        public IList<Trail> Trail { get; set; }

        public string MainPhotoUrl { get; set; }
    }

    public class Trail : BaseDataObject
    {
        public int SequenceId { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTime RecordedTimeStamp { get; set; }

        public Dictionary<String, String> OBDData { get; set; }
    }

    public class Telemetry : BaseDataObject
    {
        public string Key { get; set; }


        public string Value { get; set; }
    }


    //TODO: Conflicting with structure discussed with Haishi - commenting out for now and can discuss in next sync meeting
    //public class Point : BaseDataObject
    //{
    //    public string Elevation { get; set; }

    //    public DateTime Time { get; set; }

    //    public string Latitude { get; set; }

    //    public string Longitude { get; set; }
    //}


    //public class Trip : BaseDataObject
    //{
    //    public string Name { get; set; }

    //    public List<Point> Points { get; set; }

    //    public double Distance { get; set; }

    //    public Feedback Feedback { get; set; }

    //    public DateTime StartTime { get; set; }

    //    public DateTime EndTime { get; set; }

    //    public double MPG { get; set; }

    //    public double Gallons { get; set; }

    //    public double Cost { get; set; }
    //}
}
