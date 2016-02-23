using System;

using Foundation;
using UIKit;
using MapKit;
using CoreLocation;

namespace MyTrips.iOS
{
    public partial class TripMapView : MKMapView
    {
        public TripMapView (IntPtr handle) : base (handle)
        {
        }
    }
}