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
        public int Ranking { get; set; }

        /// <summary>
        /// Gets or sets the total distance. Stored in Miles
        /// </summary>
        /// <value>The total distance.</value>
        public double TotalDistance {get;set;}

        public long TotalTrips { get; set; }

        public long TotalTime { get; set; }

        public long HardStops { get; set; }

        public long HardAccelerations { get; set; }

        /// <summary>
        /// Gets or sets the fuel consumption. Stored in gallons
        /// </summary>
        /// <value>The fuel consumption.</value>
        public double FuelConsumption { get; set; }

        /// <summary>
        /// Gets or sets the max speed. Stored in km/h
        /// </summary>
        /// <value>The max speed.</value>
        public double MaxSpeed { get; set; }

        public List<Device> Devices { get; set; }
    }

    public class Device : BaseDataObject
    {
        public string Name { get; set; }
    }
}