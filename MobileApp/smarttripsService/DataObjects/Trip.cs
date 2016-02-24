using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace smarttripsService.DataObjects
{
    public class Trip: EntityData
    {
        public string Name { get; set; }
        public string UserId { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsComplete { get; set; }
        public float StartingLatitude { get; set; }
        public float StartingLongitude { get; set; }
        public byte Rate { get; set; }
    }
}