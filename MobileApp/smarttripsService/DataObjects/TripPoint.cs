using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace smarttripsService.DataObjects
{
    public class TripPoint: EntityData
    {
        public string TripId { get; set; }
        public int Sequence { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public DateTime DateTime { get; set; }
    }
}