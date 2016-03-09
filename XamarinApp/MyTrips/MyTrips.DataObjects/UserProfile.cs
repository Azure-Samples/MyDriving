using System;
using System.Collections.Generic;

namespace MyTrips.DataObjects
{
    public class UserProfile : BaseDataObject
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserId { get; set; }

        public string ProfilePictureUri { get; set; }

        public int Rating {get;set;}

        public double TotalDistance {get;set;}

        public long TotalTrips { get; set; }

        public long TotalTime { get; set; }

        public long HardStops { get; set; }

        public long HardAcceleration { get; set; }

        public double FuelConsumption { get; set; }

        public double MaxSpeed { get; set; }
        public List<string> Devices { get; set; }
    }
}