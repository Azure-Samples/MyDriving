using System;

using CoreLocation;
using MapKit;

namespace MyTrips.iOS
{
    public partial class TripMapView : MKMapView
    {
		MKPolyline routeOverlay;

		public TripMapView(IntPtr handle) : base(handle)
		{
		}

		public void DrawRoute(CLLocationCoordinate2D[] route)
		{
			routeOverlay = MKPolyline.FromCoordinates(route);
			AddOverlay(routeOverlay);
		}

		public override void RemoveOverlay(IMKOverlay overlay)
		{
			base.RemoveOverlay(overlay);

			routeOverlay = null;
		}
    }
}