using System;

using Foundation;
using UIKit;
using MapKit;
using CoreLocation;

namespace MyTrips.iOS
{
    public partial class TripMapView : MKMapView
    {
		public TripMapView(IntPtr handle) : base(handle)
		{
		}

		public void DrawRoute(CLLocationCoordinate2D[] route)
		{
			Console.WriteLine(route.Length);

			foreach (var r in route)
				Console.WriteLine("{0}, {1}", r.Latitude, r.Longitude);
			
			var routeOverlay = MKPolyline.FromCoordinates(route);
			AddOverlay(routeOverlay);
		}
    }
}