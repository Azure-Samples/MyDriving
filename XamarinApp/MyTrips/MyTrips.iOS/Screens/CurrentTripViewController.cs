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
		MapViewDelegate mapDelegate;

		CurrentTripViewModel ViewModel { get; set; }

		public CurrentTripViewController (IntPtr handle) : base (handle)
		{
		}

		// TODO: Add start/endpoints
		public async override void ViewDidLoad()
		{
			base.ViewDidLoad();

			ViewModel = new CurrentTripViewModel ();
			ViewModel.Geolocator.PositionChanged += Geolocator_PositionChanged;

			// Configure MKMapView
			mapDelegate = new MapViewDelegate();
			tripMapView.Delegate = mapDelegate;

			var currentLocation = await ViewModel.Geolocator.GetPositionAsync();
			var currentCoordinate = new CLLocationCoordinate2D(currentLocation.Latitude, currentLocation.Longitude);
			tripMapView.Camera.CenterCoordinate = currentCoordinate;

			tripMapView.ShowsUserLocation = false;
			tripMapView.Camera.Altitude = 5000;

			// TODO: Configure this to update if driving, but not recording.
			currentLocationAnnotation = new CarAnnotation(currentCoordinate);
			tripMapView.AddAnnotations(currentLocationAnnotation);

			recordButton.TouchUpInside += RecordButton_TouchUpInside;

			// TODO: Figure out a better way to do this - the collection will contain points that 
			// aren't part of trip - but setup for Geolocator needs to be here to update annotations
			// in the event that they haven't started a trip yet, but are driving
			await ViewModel.ExecuteStartTrackingTripCommandAsync();
		}

		void Geolocator_PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
		{
			var position = e.Position;
			var coordinate = new CLLocationCoordinate2D(position.Latitude, position.Longitude);

			UpdateCarAnnotationPosition (coordinate);

			if (ViewModel.Recording)
			{
				// Means we haven't starting tracking route yet.
				if (route == null)
				{
					route = new List<CLLocationCoordinate2D>();

					var count = ViewModel.CurrentTrip.Trail.Count;
					if (count == 0)
					{
						//TODO write an extension for this
						var firstCoordinate = coordinate;
						route.Add(firstCoordinate);
					}
					else
					{
						var firstPoint = ViewModel.CurrentTrip.Trail?[0];
						var firstCoordinate = new CLLocationCoordinate2D(firstPoint.Latitude, firstPoint.Longitude);
						route.Add(firstCoordinate);
					}
				}
			}

			// If recording, draw routes
			if (ViewModel.Recording)
			{
				route.Add(coordinate);

				// Draw updated route
				var newMKPolylineCooordinates = new CLLocationCoordinate2D[] {
					route[route.Count-1],
					route[route.Count-2]
				};

				tripMapView.DrawRoute(newMKPolylineCooordinates);
			}
		}

		async void RecordButton_TouchUpInside(object sender, EventArgs e)
		{
			// Not recording yet
			if (!ViewModel.Recording)
			{
				// TODO: Temp workaround to comment above
				ViewModel.CurrentTrip.Trail.Clear();

				// Start recording
				await ViewModel.ExecuteStartTrackingTripCommandAsync();

				// Change button text
				recordButton.SetTitle("Stop", UIControlState.Normal);
			}
			// Recording
			else
			{
				// Stop recording
				// await ViewModel.ExecuteStopTrackingTripCommandAsync();

				// Change button text
				recordButton.SetTitle("Start", UIControlState.Normal);
			}

			ViewModel.Recording = !ViewModel.Recording;
		}

		void UpdateCarAnnotationPosition(CLLocationCoordinate2D coordinate)
		{
			tripMapView.RemoveAnnotation(currentLocationAnnotation);
			currentLocationAnnotation = new CarAnnotation(coordinate);
			tripMapView.AddAnnotation(currentLocationAnnotation);

			tripMapView.Camera.CenterCoordinate = coordinate;
		}
	}
}