using System;
using Android.Gms.Maps.Model;
using MyDriving.DataObjects;
using System.Collections.Generic;
using System.Linq;

namespace MyDriving.Droid.Helpers
{
    public static class LocationExtensions
    {
        public static LatLng ToLatLng(this TripPoint point) => new LatLng(point.Latitude, point.Longitude);
        public static LatLng ToLatLng(this Plugin.Geolocator.Abstractions.Position point) => new LatLng(point.Latitude, point.Longitude);
        public static List<LatLng> ToLatLngs(this IEnumerable<TripPoint> points) => new List<LatLng>(points.Select(s => s.ToLatLng()));
    }
}

