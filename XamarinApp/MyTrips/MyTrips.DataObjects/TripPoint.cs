using System;

namespace MyTrips.DataObjects
{
    public class TripPoint: BaseDataObject
    {
        public string TripId { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double Speed { get; set; }

        public DateTime TimeStamp { get; set; }

        public int Sequence {get;set;}
    }
}