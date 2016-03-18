using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MyDriving.Interfaces
{
    public interface IGeocoder
    {
        Task<IEnumerable<Position>> GetPositionsForAddressAsync(string address);
        Task<IEnumerable<string>> GetAddressesForPositionAsync(Position position);
    }

    public class Position
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}

