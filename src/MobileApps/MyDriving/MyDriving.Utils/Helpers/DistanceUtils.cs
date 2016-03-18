// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using static System.Math;

namespace MyDriving.Utils
{
    public static class DistanceUtils
    {
        /// <summary>
        ///     Calculates the distance in miles
        /// </summary>
        /// <returns>The distance.</returns>
        /// <param name="latitudeStart">Latitude start.</param>
        /// <param name="longitudeStart">Longitude start.</param>
        /// <param name="latitudeEnd">Latitude end.</param>
        /// <param name="longitudeEnd">Longitude end.</param>
        public static double CalculateDistance(double latitudeStart, double longitudeStart,
            double latitudeEnd, double longitudeEnd)
        {
            if (latitudeEnd == latitudeStart && longitudeEnd == longitudeStart)
                return 0;

            var rlat1 = PI*latitudeStart/180.0;
            var rlat2 = PI*latitudeEnd/180.0;
            var theta = longitudeStart - longitudeEnd;
            var rtheta = PI*theta/180.0;
            var dist = Sin(rlat1)*Sin(rlat2) + Cos(rlat1)*Cos(rlat2)*Cos(rtheta);
            dist = Acos(dist);
            dist = dist*180.0/PI;
            var final = dist*60.0*1.1515;
            if (double.IsNaN(final) || double.IsInfinity(final) || double.IsNegativeInfinity(final) ||
                double.IsPositiveInfinity(final) || final < 0)
                return 0;

            return final;
        }

        public static double MilesToKilometers(double miles) => miles*1.609344;
    }
}