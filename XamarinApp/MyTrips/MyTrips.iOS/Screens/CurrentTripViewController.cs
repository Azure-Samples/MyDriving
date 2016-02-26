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
		CarAnnotation currentLocationAnnotation;
		TripMapViewDelegate mapDelegate;

		CurrentTripViewModel ViewModel { get; set; }

		public CurrentTripViewController (IntPtr handle) : base (handle)
		{
		}

		// TODO: Add start/endpoints
		public async override void ViewDidLoad()
		{
			base.ViewDidLoad();

			// Setup view model
			ViewModel = new CurrentTripViewModel ();
			ViewModel.Geolocator.PositionChanged += Geolocator_PositionChanged;
			await ViewModel.ExecuteStartTrackingTripCommandAsync();

			// Configure MKMapView
			mapDelegate = new TripMapViewDelegate();
			tripMapView.Delegate = mapDelegate;
			tripMapView.ShowsUserLocation = false;
			tripMapView.Camera.Altitude = 5000;

			// Setup button
			recordButton.TouchUpInside += RecordButton_TouchUpInside;
		}

		void Geolocator_PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
		{
			var coordinate = e.Position.ToCoordinate();
			UpdateCarAnnotationPosition (coordinate);

			if (ViewModel.Recording)
			{
				// If we already haven't starting tracking route yet, start that.
				if (route == null)
					StartTrackingRoute(coordinate);
				// Draw from last known coordinate to new coordinate.
				else
					DrawNewRouteWaypoint(coordinate);
			}
		}

		void DrawNewRouteWaypoint(CLLocationCoordinate2D coordinate)
		{
			route.Add(coordinate);

			// Draw updated route
			var newMKPolylineCooordinates = new CLLocationCoordinate2D[] {
					route[route.Count-1],
					route[route.Count-2]
				};

			tripMapView.DrawRoute(newMKPolylineCooordinates);
		}

		void StartTrackingRoute(CLLocationCoordinate2D coordinate)
		{
			route = new List<CLLocationCoordinate2D>();

			var count = ViewModel.CurrentTrip.Trail.Count;
			if (count == 0)
			{
				route.Add(coordinate);
			}
			else
			{
				var firstPoint = ViewModel.CurrentTrip.Trail?[0];
				var firstCoordinate = new CLLocationCoordinate2D(firstPoint.Latitude, firstPoint.Longitude);
				route.Add(firstCoordinate);
			}
		}

		async void RecordButton_TouchUpInside(object sender, EventArgs e)
		{
			var position = await ViewModel.Geolocator.GetPositionAsync();
			var coordinate = position.ToCoordinate();

			if (!ViewModel.Recording)
			{
				recordButton.SetTitle("Stop", UIControlState.Normal);

				// Add starting waypoint
				var annotation = new MKPointAnnotation();
				annotation.SetCoordinate (coordinate);
				annotation.Title = "A";
				tripMapView.AddAnnotation(annotation);
			}
			else
			{
				recordButton.SetTitle("Start", UIControlState.Normal);

				// Add ending waypoint
				var annotation = new MKPointAnnotation();
				annotation.SetCoordinate (coordinate);
				annotation.Title = "B";
				tripMapView.AddAnnotation(annotation);
			}

			ViewModel.Recording = !ViewModel.Recording;
		}

		void UpdateCarAnnotationPosition(CLLocationCoordinate2D coordinate)
		{
			if (currentLocationAnnotation != null)
			{
				tripMapView.RemoveAnnotation(currentLocationAnnotation);
			}

			currentLocationAnnotation = new CarAnnotation(coordinate);
			tripMapView.AddAnnotation(currentLocationAnnotation);
			tripMapView.Camera.CenterCoordinate = coordinate;
		}
	}
}