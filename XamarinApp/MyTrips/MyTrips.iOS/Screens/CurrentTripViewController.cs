using Foundation;
using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using UIKit;

using MapKit;
using CoreLocation;

using MyTrips.ViewModel;

namespace MyTrips.iOS
{
	partial class CurrentTripViewController : UIViewController
	{
		List<CLLocationCoordinate2D> route;

		CLLocationCoordinate2D locBoston = new CLLocationCoordinate2D(42.3601, -71.0589);

		CarAnnotation currentLocationAnnotation;
		MapViewDelegate mapDelegate;

		CurrentTripViewModel ViewModel { get; set; }

		public CurrentTripViewController (IntPtr handle) : base (handle)
		{
		}

		public async override void ViewDidLoad()
		{
			base.ViewDidLoad();

			mapDelegate = new MapViewDelegate();
			tripMapView.Delegate = mapDelegate;

			currentLocationAnnotation = new CarAnnotation(locBoston);
			tripMapView.AddAnnotations(currentLocationAnnotation);

			tripMapView.Camera.CenterCoordinate = new CLLocationCoordinate2D(37.797534, -122.401827);
			tripMapView.Camera.Altitude = 5000;

			// Start tracking current trip
			// TODO: Make this a button.
			ViewModel = new CurrentTripViewModel();
			await ViewModel.ExecuteStartTrackingTripCommandAsync();
			ViewModel.Geolocator.PositionChanged += Geolocator_PositionChanged;

			// Start route
			route = new List<CLLocationCoordinate2D>();
            var firstPoint = ViewModel.CurrentTrip.Trail?[0];
            if (firstPoint != null)
            {
                //TODO write an extension for this
                var firstCoordinate = new CLLocationCoordinate2D(firstPoint.Latitude, firstPoint.Longitude);
                route.Add(firstCoordinate);
            }

			tripMapView.ShowsUserLocation = false;
		}

		void Geolocator_PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
		{
			var position = e.Position;
			var coordinate = new CLLocationCoordinate2D(position.Latitude, position.Longitude);

			// Update annotations
			tripMapView.RemoveAnnotation(currentLocationAnnotation);

			currentLocationAnnotation = new CarAnnotation(coordinate);

			tripMapView.AddAnnotation(currentLocationAnnotation);

			// Move map camera
			tripMapView.Camera.CenterCoordinate = coordinate;

			// Add coordinate to route
			route.Add(coordinate);

			// Draw updated route
			var newMKPolylineCooordinates = new CLLocationCoordinate2D[] {
				route[route.Count-1],
				route[route.Count-2]
			};

			tripMapView.DrawRoute(newMKPolylineCooordinates);
		}
	}
}