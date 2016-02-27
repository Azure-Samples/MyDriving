using System;
using System.Collections.Generic;

using CoreAnimation;
using CoreLocation;
using Foundation;
using MapKit;
using UIKit;

using MyTrips.ViewModel;

namespace MyTrips.iOS
{
	partial class CurrentTripViewController : UIViewController
	{
		List<CLLocationCoordinate2D> route;
		CarAnnotation currentLocationAnnotation;
		TripMapViewDelegate mapDelegate;

		public PastTripsDetailViewModel PastTripsDetailViewModel { get; set; }

		CurrentTripViewModel ViewModel { get; set; }

		public CurrentTripViewController (IntPtr handle) : base (handle)
		{
		}

		public async override void ViewDidLoad()
		{
			base.ViewDidLoad();
			UpdateRecordButton(false);

			wayPointA.Layer.CornerRadius = wayPointA.Frame.Width / 2;
			wayPointA.Layer.BorderWidth = 2;
			wayPointA.Layer.BorderColor = UIColor.White.CGColor;

			wayPointB.Layer.CornerRadius = wayPointB.Frame.Width / 2;
			wayPointB.Layer.BorderWidth = 2;
			wayPointB.Layer.BorderColor = UIColor.White.CGColor;

			if (PastTripsDetailViewModel == null)
			{
				// Setup view model
				ViewModel = new CurrentTripViewModel();
				ViewModel.Geolocator.PositionChanged += Geolocator_PositionChanged;
				await ViewModel.ExecuteStartTrackingTripCommandAsync();

				// Configure MKMapView
				mapDelegate = new TripMapViewDelegate(UIColor.Red, 0.4);
				tripMapView.Delegate = mapDelegate;
				tripMapView.ShowsUserLocation = false;
				tripMapView.Camera.Altitude = 5000;

				// Setup button
				recordButton.Hidden = true;
				recordButton.Layer.CornerRadius = recordButton.Frame.Width / 2;
				recordButton.Layer.MasksToBounds = true;
				recordButton.Layer.BorderColor = UIColor.White.CGColor;
				recordButton.Layer.BorderWidth = 2;
				recordButton.TouchUpInside += RecordButton_TouchUpInside;

				// Hide slider waypoints
				wayPointA.Hidden = true;
				wayPointB.Hidden = true;

				// Hide trip slider
				tripSlider.Hidden = true;
			}
			else
			{
				// Update navigation bar title
				NavigationItem.Title = PastTripsDetailViewModel.Title;

				var count = PastTripsDetailViewModel.Trip.Trail.Count;

				// Setup map
				mapDelegate = new TripMapViewDelegate(UIColor.Blue, 0.6);
				tripMapView.Delegate = mapDelegate;
				tripMapView.ShowsUserLocation = false;
				tripMapView.Camera.Altitude = 10000;
				tripMapView.SetVisibleMapRect(MKPolyline.FromCoordinates(PastTripsDetailViewModel.Trip.Trail.ToCoordinateArray()).BoundingMapRect, new UIEdgeInsets(25, 25, 25, 25), false);

				// Draw route
				tripMapView.DrawRoute(PastTripsDetailViewModel.Trip.Trail.ToCoordinateArray ());

				// Draw endpoints
				var startEndpoint = new WaypointAnnotation (PastTripsDetailViewModel.Trip.Trail[0].ToCoordinate (), "A");
				tripMapView.AddAnnotation(startEndpoint);

				var endEndpoint = new WaypointAnnotation(PastTripsDetailViewModel.Trip.Trail[count - 1].ToCoordinate (), "B");
				tripMapView.AddAnnotation(endEndpoint);

				var carCoordinate = PastTripsDetailViewModel.Trip.Trail[count / 2].ToCoordinate ();
				currentLocationAnnotation = new CarAnnotation(carCoordinate, "Blue");
				tripMapView.AddAnnotation(currentLocationAnnotation);

				// Hide record button
				recordButton.Hidden = true;
				wayPointA.Hidden = false;
				wayPointA.Hidden = false;

				ConfigureSlider();
			}
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			if (recordButton.Hidden == true && PastTripsDetailViewModel == null)
			{
				recordButton.Pop(0.5, 0, 1);
			}
		}

		void Geolocator_PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
		{
			var coordinate = e.Position.ToCoordinate();
			UpdateCarAnnotationPosition (coordinate);

			if (ViewModel.IsRecording)
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

			if (!ViewModel.IsRecording)
			{
				// Add starting waypoint
				var startEndpoint = new WaypointAnnotation (coordinate, "A");
				tripMapView.AddAnnotation(startEndpoint);

				UpdateRecordButton(true);
				ResetDetailsView();
			}
			else
			{
				UpdateRecordButton(false);

				// Add ending waypoint
				var endEndpoint = new WaypointAnnotation(coordinate, "B");
				tripMapView.AddAnnotation(endEndpoint);
			}

			ViewModel.IsRecording = !ViewModel.IsRecording;
		}

		void TripSlider_ValueChanged(object sender, EventArgs e)
		{
			// Move car to coordinate
			var value = (int)tripSlider.Value;
			var coordinate = PastTripsDetailViewModel.Trip.Trail[value].ToCoordinate();
			UpdateCarAnnotationPosition(coordinate);
		}

		void UpdateRecordButton(bool isRecording)
		{
			//Corner Radius
			var radiusAnimation = CABasicAnimation.FromKeyPath("cornerRadius");
			radiusAnimation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseIn);
			radiusAnimation.From = NSNumber.FromNFloat(recordButton.Layer.CornerRadius);

			//Border Thickness
			var borderAnimation = CABasicAnimation.FromKeyPath("borderWidth");
			borderAnimation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseIn);
			radiusAnimation.From = NSNumber.FromNFloat(recordButton.Layer.BorderWidth);

			//Animation Group
			var animationGroup = CAAnimationGroup.CreateAnimation();
			animationGroup.Animations = new CAAnimation[] { radiusAnimation, borderAnimation };
			animationGroup.Duration = 0.6;
			animationGroup.FillMode = CAFillMode.Forwards;

			recordButton.Layer.CornerRadius = isRecording ? 4 : recordButton.Frame.Width / 2;
			recordButton.Layer.BorderWidth = isRecording ? 2 : 3;

			recordButton.Layer.AddAnimation(animationGroup, "borderChanges");
		}

		void UpdateCarAnnotationPosition(CLLocationCoordinate2D coordinate)
		{
			if (currentLocationAnnotation != null)
			{
				tripMapView.RemoveAnnotation(currentLocationAnnotation);
			}

			if (ViewModel != null)
			{
				if (ViewModel.IsRecording)
				{
					currentLocationAnnotation = new CarAnnotation(coordinate, "Red");
				}
				else
				{
					currentLocationAnnotation = new CarAnnotation(coordinate, "Blue");
				}
			}
			else
			{
				currentLocationAnnotation = new CarAnnotation(coordinate, "Blue");
			}

			tripMapView.AddAnnotation(currentLocationAnnotation);
			tripMapView.Camera.CenterCoordinate = coordinate;
		}

		void ResetDetailsView()
		{
			var duration = 0.5f;

			lblMpg.Text = "0";
			lblMpg.Pop(duration, 0, 1);

			lblGallons.Text = "0";
			lblGallons.Pop(duration, 0, 1);

			lblDistance.Text = "0";
			lblDistance.Pop(duration, 0, 1);

			lblDuration.Text = "0:00";
			lblDuration.Pop(duration, 0, 1);

			lblCost.Text = "$0.00";
			lblCost.Pop(duration, 0, 1);
		}

		void ConfigureSlider()
		{
			var dataPoints = PastTripsDetailViewModel.Trip.Trail.Count - 1;
			tripSlider.MinValue = 0;
			tripSlider.MaxValue = dataPoints;
			tripSlider.Value = PastTripsDetailViewModel.Trip.Trail.Count / 2;

			tripSlider.ValueChanged += TripSlider_ValueChanged;
		}
	}
}