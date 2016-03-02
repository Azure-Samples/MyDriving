using System;

namespace MyTrips.DataObjects
{
    public class Photo : BaseDataObject
    {
        public string TripId { get; set; }

        public string PhotoUrl { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}

