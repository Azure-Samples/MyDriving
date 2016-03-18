using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VINParser
{
    public class CarInfo
    {
        public int Year { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public CarType CarType { get; set; }
    }
}
