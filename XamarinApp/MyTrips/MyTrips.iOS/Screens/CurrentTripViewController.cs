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

		CarAnnotation currentLocationAnnotation;

		public CurrentTripViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			tripMapView.Delegate = new MapViewDelegate ();

			currentLocationAnnotation = new CarAnnotation(locBoston);
			tripMapView.AddAnnotations(currentLocationAnnotation);

			tripMapView.Camera.CenterCoordinate = new CLLocationCoordinate2D(37.797534, -122.401827);
			tripMapView.Camera.Altitude = 5000;

			// TODO: Add a graceful way to handle permission issues.
			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0) == true) {
				locationManager.RequestAlwaysAuthorization();
			}

			tripMapView.ShowsUserLocation = true;
		}

		public void UserLocationChanged(CLLocationCoordinate2D newLocation)
		{
			// Update annotations
			tripMapView.RemoveAnnotation(currentLocationAnnotation);

			currentLocationAnnotation = new CarAnnotation(newLocation);

			tripMapView.AddAnnotation(currentLocationAnnotation);

			// Move map camera
			tripMapView.Camera.CenterCoordinate = newLocation;
			tripMapView.Camera.Altitude = 5000;
		}
	}
}