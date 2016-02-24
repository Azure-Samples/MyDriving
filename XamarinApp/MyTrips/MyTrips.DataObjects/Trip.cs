using System;
using System.Collections.Generic;
using System.Text;

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

        public string TripId { get; set; }

        public string UserId { get; set; }

        //TODO: need to have logic in app that actually calculates this; or maybe be calculated on backend
        public string TotalDistance { get; set; }

        //TODO: need to have logic in app that actually calculates this 
        public string DaysSinceRecording { get; set; }

        public List<Trail> Trail { get; set; }
    }

    public class Trail : BaseDataObject
    {
        public Trail()
        {
            this.Telemetry = new List<Telemetry>();
        }

        public int TrailId { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTime TimeStamp { get; set; }

        public List<Telemetry> Telemetry { get; set; }
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
