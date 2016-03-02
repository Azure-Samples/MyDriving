using System;
using static System.Math;

namespace MyTrips.Utils
{
    public static class DistanceUtils
    {
        /// <summary>
        /// Calculates the distance in miles
        /// </summary>
        /// <returns>The distance.</returns>
        /// <param name="latitudeStart">Latitude start.</param>
        /// <param name="longitudeStart">Longitude start.</param>
        /// <param name="latitudeEnd">Latitude end.</param>
        /// <param name="longitudeEnd">Longitude end.</param>
        public static double CalculateDistance(double latitudeStart, double longitudeStart,
                                               double latitudeEnd, double longitudeEnd)
        {
            var rlat1 = PI * latitudeStart / 180.0;
            var rlat2 = PI * latitudeEnd / 180.0;
            var theta = longitudeStart - longitudeEnd;
            var rtheta = PI * theta / 180.0;
            var dist = Sin(rlat1) * Sin(rlat2) + Cos(rlat1) * Cos(rlat2) * Cos(rtheta);
            dist = Acos(dist);
            dist = dist * 180.0 / PI;
            return dist * 60.0 * 1.1515;
        }

        public static double MilesToKilometers(double miles) => miles * 1.609344;
    }
}

