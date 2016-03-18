using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VINParser
{
    public class MakeMap
    {
        public int StartDigit { get; set; }
        public int MaxLength { get; set; }
        public Dictionary<string,CarInfo> Map { get; set; }
    }
}
