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
			ConfigureRouteDrawing();	
        }

		void ConfigureRouteDrawing()
		{
			var routeOverlay = MKPolyline.FromCoordinates(route);
			AddOverlay(routeOverlay);
		}

		public CLLocationCoordinate2D[] route = new CLLocationCoordinate2D[] {
			new CLLocationCoordinate2D(37.797534, -122.401827),
			new CLLocationCoordinate2D(37.797510, -122.402060),
			new CLLocationCoordinate2D(37.790269, -122.400589),
			new CLLocationCoordinate2D(37.790265, -122.400474),
			new CLLocationCoordinate2D(37.790228, -122.400391),
			new CLLocationCoordinate2D(37.790126, -122.400360),
			new CLLocationCoordinate2D(37.789250, -122.401451),
			new CLLocationCoordinate2D(37.788440, -122.400396),
			new CLLocationCoordinate2D(37.787999, -122.399780),
			new CLLocationCoordinate2D(37.786736, -122.398202),
			new CLLocationCoordinate2D(37.786345, -122.397722),
			new CLLocationCoordinate2D(37.785983, -122.397295),
			new CLLocationCoordinate2D(37.785559, -122.396728),
			new CLLocationCoordinate2D(37.780624, -122.390541),
			new CLLocationCoordinate2D(37.777113, -122.394983),
			new CLLocationCoordinate2D(37.776831, -122.394627)
		};
    }
}