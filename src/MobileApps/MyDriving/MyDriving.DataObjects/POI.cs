using System;
using System.Collections.Generic;
using System.Text;

namespace MyDriving.DataObjects
{
    public class POI: BaseDataObject
    {
        public string TripId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public POIType POIType { get; set; }
        public DateTime Timestamp { get; set; }

    }
}
