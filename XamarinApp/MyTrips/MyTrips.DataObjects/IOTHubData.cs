using System;
using System.Collections.Generic;
using System.Text;

namespace MyTrips.DataObjects
{
    public class IOTHubData : BaseDataObject
    {
        public string TripName { get; set; }

        public string UserId { get; set; }

        public DateTime TimeStamp { get; set; }

        public string OBDData { get; set; }

        public string TripPoints { get; set; }
    }
}
