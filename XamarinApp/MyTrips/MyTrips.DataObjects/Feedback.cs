using System;
using System.Collections.Generic;
using System.Text;

namespace MyTrips.DataObjects
{
    public class Feedback : BaseDataObject 
    {
        public string RouteId { get; set; }
        public string TripId { get; set; }
        public int Rating { get; set; }
    }
}
