using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

using MapKit;
using CoreLocation;

namespace MyTrips.iOS
{
	partial class CurrentTripViewController : UIViewController
	{
		CLLocationCoordinate2D locBoston = new CLLocationCoordinate2D(42.3601, -71.0589);
		CLLocationManager locationManager = new CLLocationManager();

		public CurrentTripViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			tripMapView.Delegate = new MapViewDelegate ();
			tripMapView.AddAnnotations (new CarAnnotation (locBoston));

			tripMapView.Camera.CenterCoordinate = new CLLocationCoordinate2D(37.797534, -122.401827);
			tripMapView.Camera.Altitude = 5000;

			// TODO: Add a graceful way to handle permission issues.
			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0) == true) {
				locationManager.RequestAlwaysAuthorization ();
			}

			tripMapView.ShowsUserLocation = true;
		}
	}
}