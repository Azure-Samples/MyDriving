using System;
namespace MyTrips.DataObjects
{
    public class Tip : BaseDataObject
    {
        public string TripId { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double Information { get; set;}
    }
}

