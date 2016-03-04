using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CoreAnimation;
using CoreLocation;
using Foundation;
using MapKit;
using UIKit;

using MyTrips.ViewModel;

using Plugin.Permissions;
using Plugin.Permissions.Abstractions;


namespace MyTrips.iOS
{
	partial class CurrentTripViewController : UIViewController
	{
		List<CLLocationCoordinate2D> route;
		CarAnnotation currentLocationAnnotation;
		TripMapViewDelegate mapDelegate;

		CurrentTripViewModel CurrentTripViewModel { get; set; }

		public PastTripsDetailViewModel PastTripsDetailViewModel { get; set; }

		public CurrentTripViewController (IntPtr handle) : base (handle)
		{
		}

		public async override void ViewDidLoad()
		{
			base.ViewDidLoad();

			NavigationItem.RightBarButtonItem = null;

			if (PastTripsDetailViewModel == null)
			{
				await ConfigureCurrentTripUserInterface();
			}
			else
			{
				ConfigurePastTripUserInterface();
			}
		}

		public override async void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			if (CurrentTripViewModel != null && !CurrentTripViewModel.IsRecording)
			{
				await CurrentTripViewModel.ExecuteStartTrackingTripCommandAsync();
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

		public override async void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);

			if (CurrentTripViewModel != null && !CurrentTripViewModel.IsRecording)
			{
				await CurrentTripViewModel.ExecuteStopTrackingTripCommandAsync();
			}
		}

		#region Current Trip User Interface Logic
		async Task ConfigureCurrentTripUserInterface()
		{
			// Configure map
			mapDelegate = new TripMapViewDelegate(UIColor.Red, 0.6);
			tripMapView.Delegate = mapDelegate;
			tripMapView.ShowsUserLocation = false;
			tripMapView.Camera.Altitude = 5000;

			// Setup record button
			recordButton.Hidden = true;
			recordButton.Layer.CornerRadius = recordButton.Frame.Width / 2;
			recordButton.Layer.MasksToBounds = true;
			recordButton.Layer.BorderColor = "5C5C5C".ToUIColor().CGColor;
			recordButton.Layer.BorderWidth = 1;
			recordButton.TouchUpInside += RecordButton_TouchUpInside;

			// Hide slider
			tripSlider.Hidden = true;
			wayPointA.Hidden = true;
			wayPointB.Hidden = true;

			UpdateRecordButton(false);

			// Setup view model
			CurrentTripViewModel = new CurrentTripViewModel();
			CurrentTripViewModel.Geolocator.PositionChanged += Geolocator_PositionChanged;

			// Start tracking user location, pending permission from user.
			await CurrentTripViewModel.ExecuteStartTrackingTripCommandAsync().ContinueWith(async (task) =>
			{
				// If we don't have permission from the user, prompt a dialog requesting permission.
				await PromptPermissionsChangeDialog();
			});
		}

		void ResetMapViewState()
		{
			if (tripMapView.Overlays != null)
			{
				tripMapView.RemoveOverlays(tripMapView.Overlays);
			}

			tripMapView.RemoveAnnotations(tripMapView.Annotations);
			route = null;
		}

		void ResetTripInfoView()
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

		async Task PromptPermissionsChangeDialog()
		{
			var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
			if (status == PermissionStatus.Denied)
			{
				InvokeOnMainThread(() =>
				{
					var alertController = UIAlertController.Create("Location Permission Denied", "Tracking your location is required to record trips. Visit the Settings app to change the permission status.", UIAlertControllerStyle.Alert);
					alertController.AddAction(UIAlertAction.Create("Change Permission", UIAlertActionStyle.Default, (obj) =>
					{
						var url = NSUrl.FromString(UIApplication.OpenSettingsUrlString);
						UIApplication.SharedApplication.OpenUrl(url);
					}));

					alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, null));

					PresentViewController(alertController, true, null);
				});
			}
		}

		void TakePhotoButton_Clicked(object sender, EventArgs e)
		{
			if (!CurrentTripViewModel.IsBusy && CurrentTripViewModel.IsRecording)
				CurrentTripViewModel?.TakePhotoCommand.Execute(null);
		}

		async void RecordButton_TouchUpInside(object sender, EventArgs e)
		{
			var position = await CurrentTripViewModel.Geolocator.GetPositionAsync();
			var coordinate = position.ToCoordinate();

			if (!CurrentTripViewModel.IsRecording)
			{
				if (NavigationItem.RightBarButtonItem == null)
					NavigationItem.SetRightBarButtonItem(takePhotoButton, true);

				NavigationItem.RightBarButtonItem.Clicked += TakePhotoButton_Clicked;

				UpdateRecordButton(true);
				ResetTripInfoView();
			}
			else
			{
				UpdateRecordButton(false);

				NavigationItem.RightBarButtonItem.Clicked -= TakePhotoButton_Clicked;
				NavigationItem.SetRightBarButtonItem(null, true);
			}

			// Add start or end waypoint
			var endpoint = !CurrentTripViewModel.IsRecording ? "A" : "B";
			var annotation = new WaypointAnnotation(coordinate, endpoint);
			tripMapView.AddAnnotation(annotation);

			if (CurrentTripViewModel.IsRecording)
			{
				ResetMapViewState();
				await CurrentTripViewModel.StopRecordingTripAsync();
				NSNotificationCenter.DefaultCenter.PostNotificationName("RefreshPastTripsTable", null);
			}
			else
			{
				await CurrentTripViewModel.StartRecordingTripAsync();
			}
		}

		void Geolocator_PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
		{
			var coordinate = e.Position.ToCoordinate();
			UpdateCarAnnotationPosition (coordinate);

			if (CurrentTripViewModel.IsRecording)
			{
				// Update trip information
				lblDuration.Text = CurrentTripViewModel.ElapsedTime;
				lblDistance.Text = CurrentTripViewModel.CurrentTrip.TotalDistanceNoUnits;

				// If we already haven't starting tracking route yet, start that.
				if (route == null)
					StartTrackingRoute(coordinate);
				// Draw from last known coordinate to new coordinate.
				else
					DrawNewRouteWaypoint(coordinate);
			}
		}

		void StartTrackingRoute(CLLocationCoordinate2D coordinate)
		{
			route = new List<CLLocationCoordinate2D>();

			var count = CurrentTripViewModel.CurrentTrip.Trail.Count;
			if (count == 0)
			{
				route.Add(coordinate);
			}
			else
			{
				var firstPoint = CurrentTripViewModel.CurrentTrip.Trail?[0];
				var firstCoordinate = new CLLocationCoordinate2D(firstPoint.Latitude, firstPoint.Longitude);
				route.Add(firstCoordinate);
			}
		}
		#endregion

		#region Past Trip User Interface Logic
		void ConfigurePastTripUserInterface()
		{
			NavigationItem.Title = PastTripsDetailViewModel.Title;

			var coordinateCount = PastTripsDetailViewModel.Trip.Trail.Count;

			// Setup map
			mapDelegate = new TripMapViewDelegate(UIColor.Blue, 0.6);
			tripMapView.Delegate = mapDelegate;
			tripMapView.ShowsUserLocation = false;
			tripMapView.Camera.Altitude = 5000;
			tripMapView.SetVisibleMapRect(MKPolyline.FromCoordinates(PastTripsDetailViewModel.Trip.Trail.ToCoordinateArray()).BoundingMapRect, new UIEdgeInsets(25, 25, 25, 25), false);

			// Draw endpoints
			var startEndpoint = new WaypointAnnotation(PastTripsDetailViewModel.Trip.Trail[0].ToCoordinate(), "A");
			tripMapView.AddAnnotation(startEndpoint);

			var endEndpoint = new WaypointAnnotation(PastTripsDetailViewModel.Trip.Trail[coordinateCount - 1].ToCoordinate(), "B");
			tripMapView.AddAnnotation(endEndpoint);

			// Draw route
			tripMapView.DrawRoute(PastTripsDetailViewModel.Trip.Trail.ToCoordinateArray());

			// Draw car
			var carCoordinate = PastTripsDetailViewModel.Trip.Trail[coordinateCount / 2].ToCoordinate();
			currentLocationAnnotation = new CarAnnotation(carCoordinate, UIColor.Blue);
			tripMapView.AddAnnotation(currentLocationAnnotation);

			ConfigureSlider();
			ConfigureWayPointButtons();

			// Hide record button
			recordButton.Hidden = true;

			// Show slider 
			sliderView.Hidden = false;

			startTimeLabel.Hidden = false;
			endTimeLabel.Hidden = false;
			startTimeLabel.Text = PastTripsDetailViewModel.Trip.StartTimeDisplay;
			endTimeLabel.Text = PastTripsDetailViewModel.Trip.EndTimeDisplay;
		}

		void ConfigureSlider()
		{
			var dataPoints = PastTripsDetailViewModel.Trip.Trail.Count - 1;
			tripSlider.MinValue = 0;
			tripSlider.MaxValue = dataPoints;
			tripSlider.Value = PastTripsDetailViewModel.Trip.Trail.Count / 2;

			tripSlider.ValueChanged += TripSlider_ValueChanged;
		}

		void ConfigureWayPointButtons()
		{
			wayPointA.Layer.CornerRadius = wayPointA.Frame.Width / 2;
			wayPointA.Layer.BorderWidth = 2;
			wayPointA.Layer.BorderColor = UIColor.White.CGColor;
			wayPointA.TouchUpInside += delegate 
			{
				tripSlider.Value = 0;
				TripSlider_ValueChanged(this, null);

			};

			wayPointB.Layer.CornerRadius = wayPointB.Frame.Width / 2;
			wayPointB.Layer.BorderWidth = 2;
			wayPointB.Layer.BorderColor = UIColor.White.CGColor;
			wayPointB.TouchUpInside += delegate 
			{
				tripSlider.Value = tripSlider.MaxValue;
				TripSlider_ValueChanged(this, null);
			};
		}

		void TripSlider_ValueChanged(object sender, EventArgs e)
		{
			// Move car to coordinate
			var value = (int)tripSlider.Value;
			var coordinate = PastTripsDetailViewModel.Trip.Trail[value].ToCoordinate();
			UpdateCarAnnotationPosition(coordinate);
		}
		#endregion

		#region Shared User Interface Logic
		void UpdateCarAnnotationPosition(CLLocationCoordinate2D coordinate)
		{
			if (currentLocationAnnotation != null)
			{
				tripMapView.RemoveAnnotation(currentLocationAnnotation);
			}

			var color = CurrentTripViewModel != null && CurrentTripViewModel.IsRecording ? UIColor.Red : UIColor.Blue;
			currentLocationAnnotation = new CarAnnotation(coordinate, color);

			tripMapView.AddAnnotation(currentLocationAnnotation);
			tripMapView.Camera.CenterCoordinate = coordinate;
		}

		void DrawNewRouteWaypoint(CLLocationCoordinate2D coordinate)
		{
			route.Add(coordinate);

			if (tripMapView.Overlays != null)
				tripMapView.RemoveOverlays(tripMapView.Overlays);

			tripMapView.DrawRoute(route.ToArray());
		}
		#endregion
	}
}