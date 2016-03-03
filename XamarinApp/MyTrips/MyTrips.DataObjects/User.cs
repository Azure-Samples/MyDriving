using System;
using System.Collections.Generic;

namespace MyTrips.DataObjects
{
    public class User : BaseDataObject
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserId { get; set; }
        public string ProfilePictureUri { get; set; }
        public double TotalDistance {get;set;}
        public int Rating {get;set;}

        public double AverageFuelConsumption { get; set; }

        public double AverageSpeed {get;set;}

        public double TotalEmissions { get; set; }

        public List<string> Devices { get; set; }
    }
}