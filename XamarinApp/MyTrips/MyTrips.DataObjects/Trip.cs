using System;
using System.Collections.Generic;
using System.Text;

namespace MyTrips.DataObjects
{
    public class Point : BaseDataObject
    {
        public string Elevation { get; set; }

        public DateTime Time { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }


    public class Trip : BaseDataObject
    {
        public string Name { get; set; }

		public List<Point> Points { get; set; }

        public double Distance { get; set; }

        public Feedback Feedback { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public double MPG { get; set; }

        public double Gallons { get; set; }

        public double Cost { get; set; }
    }
}
