// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Globalization;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreAnimation;
using CoreLocation;
using Foundation;
using MapKit;
using UIKit;
using MyDriving.DataObjects;
using MyDriving.ViewModel;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using MyDriving.Utils;
using Microsoft.WindowsAzure.MobileServices;
using MyDriving.iOS.Helpers;
using MyDriving.AzureClient;

namespace MyDriving.iOS
{
    partial class CurrentTripViewController : UIViewController
    {
        CarAnnotation currentLocationAnnotation;
        TripMapViewDelegate mapDelegate;
        List<CLLocationCoordinate2D> route;

        public CurrentTripViewController(IntPtr handle) : base(handle)
        {
        }

        public CurrentTripViewModel CurrentTripViewModel { get; set; }
        public PastTripsDetailViewModel PastTripsDetailViewModel { get; set; }

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationItem.RightBarButtonItem = null;

            if (PastTripsDetailViewModel == null)
                await ConfigureCurrentTripUserInterface();
            else
                await ConfigurePastTripUserInterface();
        }

        public override async void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            PopRecordButtonAnimation();

			if (CurrentTripViewModel != null)
			{
                await CurrentTripViewModel.ExecuteStartTrackingTripCommandAsync().ContinueWith(async (task) =>
				{
					// If we don't have permission from the user, prompt a dialog requesting permission.
					await PromptPermissionsChangeDialog();
				});

				if (!CurrentTripViewModel.Geolocator.IsGeolocationEnabled)
				{
					tripMapView.Camera.CenterCoordinate = new CLLocationCoordinate2D(47.6204, -122.3491);
					tripMapView.Camera.Altitude = 5000;
				}
			}
        }

        #region Current Trip User Interface Logic

        async Task ConfigureCurrentTripUserInterface()
        {
            // Configure map
            mapDelegate = new TripMapViewDelegate(true);
            tripMapView.Delegate = mapDelegate;
            tripMapView.ShowsUserLocation = false;
            tripMapView.Camera.Altitude = 5000;

			// Setup record button
			recordButton.Hidden = false;
            recordButton.Layer.CornerRadius = recordButton.Frame.Width/2;
            recordButton.Layer.MasksToBounds = true;
            recordButton.Layer.BorderColor = UIColor.White.CGColor;
            recordButton.Layer.BorderWidth = 0;
            recordButton.TouchUpInside += RecordButton_TouchUpInside;

            // Hide slider
            tripSlider.Hidden = true;
            wayPointA.Hidden = true;
            wayPointB.Hidden = true;

            UpdateRecordButton(false);
            tripInfoView.Alpha = 0;
            ResetTripInfoView();

            CurrentTripViewModel = new CurrentTripViewModel();
            CurrentTripViewModel.Geolocator.PositionChanged += Geolocator_PositionChanged;
                 
        }

        void AnimateTripInfoView()
        {
            tripInfoView.FadeIn(0.3, 0);
        }

        void ResetMapViewState()
        {
            InvokeOnMainThread(() =>
            {
                route = null;
                tripMapView.RemoveAnnotations(tripMapView.Annotations);

                if (tripMapView.Overlays != null && tripMapView.Overlays.Length > 0)
                {
                    tripMapView.RemoveOverlays(tripMapView.Overlays[0]);
                }
            });
        }

        void ResetTripInfoView()
        {
            labelOneValue.Text = "N/A";
            labelTwoValue.Text = "0";
            labelThreeValue.Text = "0:00";
            labelFourValue.Text = "N/A";
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
            animationGroup.Animations = new CAAnimation[] {radiusAnimation, borderAnimation};
            animationGroup.Duration = 0.6;
            animationGroup.FillMode = CAFillMode.Forwards;

            recordButton.Layer.CornerRadius = isRecording ? 4 : recordButton.Frame.Width/2;
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
                    var alertController = UIAlertController.Create("Location Permission Denied",
                        "Tracking your location is required to record trips. Visit the Settings app to change the permission status.",
                        UIAlertControllerStyle.Alert);
                    alertController.AddAction(UIAlertAction.Create("Change Permission", UIAlertActionStyle.Default,
                        obj =>
                        {
                            var url = NSUrl.FromString(UIApplication.OpenSettingsUrlString);
                            UIApplication.SharedApplication.OpenUrl(url);
                        }));

                    alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, null));

                    PresentViewController(alertController, true, null);

                    tripMapView.Camera.CenterCoordinate = new CLLocationCoordinate2D(47.6204, -122.3491);
                    tripMapView.Camera.Altitude = 5000;
                });
            }
        }

        async void RecordButton_TouchUpInside(object sender, EventArgs e)
        {
            if (!CurrentTripViewModel.Geolocator.IsGeolocationEnabled)
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert(
                    "Please ensure that geolocation is enabled and permissions are allowed for MyDriving to start a recording.",
                    "Geolocation Disabled", "OK");

                return;
            }

            if (!CurrentTripViewModel.IsRecording)
            {
                if (!await CurrentTripViewModel.StartRecordingTrip())
                    return;

                UpdateRecordButton(true);
                ResetTripInfoView();
                AnimateTripInfoView();

                if (CurrentTripViewModel.CurrentTrip.HasSimulatedOBDData)
                    NavigationItem.Title = "Current Trip (Sim OBD)";

                var endpoint = "A";
                var annotation = new WaypointAnnotation(CurrentTripViewModel.CurrentPosition.ToCoordinate(), endpoint);
                tripMapView.AddAnnotation(annotation);
            }
            else
            {
                if (await CurrentTripViewModel.StopRecordingTrip())
                {
                    ResetMapViewState();
                    InvokeOnMainThread(delegate
                    {
                        mapDelegate = new TripMapViewDelegate(true);
                        tripMapView.Delegate = mapDelegate;
                    });

                    UpdateRecordButton(false);
                    tripInfoView.Alpha = 0;
                    NavigationItem.Title = "Current Trip";

                    await CurrentTripViewModel.SaveRecordingTripAsync();

                    var vc =
                        Storyboard.InstantiateViewController("tripSummaryViewController") as TripSummaryViewController;
                    vc.ViewModel = CurrentTripViewModel;
                    PresentModalViewController(vc, true);
                }
            }
        }

        void Geolocator_PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
            var coordinate = e.Position.ToCoordinate();
            UpdateCarAnnotationPosition(coordinate);

            if (CurrentTripViewModel.IsRecording)
            {
                // Update trip information
                labelOneValue.Text = CurrentTripViewModel.FuelConsumption;
                labelOneTitle.Text = CurrentTripViewModel.FuelConsumptionUnits;
                labelThreeValue.Text = CurrentTripViewModel.ElapsedTime;
                labelTwoValue.Text = CurrentTripViewModel.CurrentTrip.Distance.ToString("F");
                labelTwoTitle.Text =
                    CultureInfo.CurrentCulture.TextInfo.ToTitleCase(CurrentTripViewModel.CurrentTrip.Units.ToLower());
                labelFourValue.Text = CurrentTripViewModel.EngineLoad;

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

            var count = CurrentTripViewModel.CurrentTrip.Points.Count;
            if (count == 0)
            {
                route.Add(coordinate);
            }
            else
            {
                var firstPoint = CurrentTripViewModel.CurrentTrip.Points?[0];
                var firstCoordinate = new CLLocationCoordinate2D(firstPoint.Latitude, firstPoint.Longitude);
                route.Add(firstCoordinate);
            }
        }

        #endregion

        #region Past Trip User Interface Logic

        async Task ConfigurePastTripUserInterface()
        {
            NavigationItem.Title = PastTripsDetailViewModel.Title;
			sliderView.Hidden = false;
			tripSlider.Hidden = false;

			wayPointA.Layer.CornerRadius = wayPointA.Frame.Width / 2;
			wayPointA.Layer.BorderWidth = 2;
			wayPointA.Layer.BorderColor = UIColor.White.CGColor;

			wayPointB.Layer.CornerRadius = wayPointB.Frame.Width / 2;
			wayPointB.Layer.BorderWidth = 2;
			wayPointB.Layer.BorderColor = UIColor.White.CGColor;
            
            var success = await PastTripsDetailViewModel.ExecuteLoadTripCommandAsync(PastTripsDetailViewModel.Trip.Id);
            
            if(!success)
            {
                NavigationController.PopViewController(true);
                return;
            }
            // Setup map
            mapDelegate = new TripMapViewDelegate(false);
            tripMapView.Delegate = mapDelegate;
            tripMapView.ShowsUserLocation = false;

            if (PastTripsDetailViewModel.Trip == null || PastTripsDetailViewModel.Trip.Points == null || PastTripsDetailViewModel.Trip.Points.Count == 0)
                return;

            var coordinateCount = PastTripsDetailViewModel.Trip.Points.Count;
            // Draw endpoints
            var startEndpoint = new WaypointAnnotation(PastTripsDetailViewModel.Trip.Points[0].ToCoordinate(), "A");
            tripMapView.AddAnnotation(startEndpoint);

            var endEndpoint =
                new WaypointAnnotation(PastTripsDetailViewModel.Trip.Points[coordinateCount - 1].ToCoordinate(), "B");
            tripMapView.AddAnnotation(endEndpoint);

            // Draw route
            tripMapView.DrawRoute(PastTripsDetailViewModel.Trip.Points.ToCoordinateArray());

            // Draw car
            var carCoordinate = PastTripsDetailViewModel.Trip.Points[0];
            currentLocationAnnotation = new CarAnnotation(carCoordinate.ToCoordinate(), UIColor.Blue);
            tripMapView.AddAnnotation(currentLocationAnnotation);

            // Configure slider area
            ConfigureSlider();
            ConfigureWayPointButtons();
            ConfigurePoiAnnotations();
            recordButton.Hidden = true;

            tripMapView.SetVisibleMapRect(
                MKPolyline.FromCoordinates(PastTripsDetailViewModel.Trip.Points.ToCoordinateArray()).BoundingMapRect,
                new UIEdgeInsets(25, 25, 25, 25), false);

            tripMapView.CenterCoordinate = carCoordinate.ToCoordinate();
            UpdateTripStatistics(carCoordinate);
            NSNotificationCenter.DefaultCenter.AddObserver(new NSString("RefreshTripUnits"), HandleTripUnitsChanged);
        }

        void HandleTripUnitsChanged(NSNotification obj)
        {
            UpdateTripStatistics(PastTripsDetailViewModel.CurrentPosition);
        }

        void UpdateTripStatistics(TripPoint point)
        {
            PastTripsDetailViewModel.CurrentPosition = point;
            labelOneTitle.Text = PastTripsDetailViewModel.FuelConsumptionUnits;
            labelOneValue.Text = PastTripsDetailViewModel.FuelConsumption;

            labelTwoTitle.Text = PastTripsDetailViewModel.DistanceUnits;
            labelTwoValue.Text = PastTripsDetailViewModel.Distance;

            labelThreeTitle.Text = "Elapsed Time";
            labelThreeValue.Text = PastTripsDetailViewModel.ElapsedTime;

            labelFourTitle.Text = PastTripsDetailViewModel.SpeedUnits;
            labelFourValue.Text = PastTripsDetailViewModel.Speed;
        }

        void ConfigureSlider()
        {
            tripSlider.MinValue = 0;
            tripSlider.MaxValue = PastTripsDetailViewModel.Trip.Points.Count - 1;
            tripSlider.Value = 0;

            tripSlider.ValueChanged += TripSlider_ValueChanged;
        }

        void ConfigureWayPointButtons()
        {
            startTimeLabel.Hidden = false;
            endTimeLabel.Hidden = false;

            startTimeLabel.Text = PastTripsDetailViewModel.Trip.StartTimeDisplay;
            endTimeLabel.Text = PastTripsDetailViewModel.Trip.EndTimeDisplay;

            wayPointA.TouchUpInside += delegate
            {
                tripSlider.Value = 0;
                TripSlider_ValueChanged(this, null);
            };

            wayPointB.TouchUpInside += delegate
            {
                tripSlider.Value = tripSlider.MaxValue;
                TripSlider_ValueChanged(this, null);
            };
        }

        void ConfigurePoiAnnotations()
        {
            foreach (var poi in PastTripsDetailViewModel.POIs)
            {
                var poiAnnotation = new PoiAnnotation(poi, poi.ToCoordinate());
                tripMapView.AddAnnotation(poiAnnotation);
            }
        }

        void TripSlider_ValueChanged(object sender, EventArgs e)
        {
            var value = (int) tripSlider.Value;
            var tripPoint = PastTripsDetailViewModel.Trip.Points[value];
            UpdateCarAnnotationPosition(tripPoint.ToCoordinate());

            UpdateTripStatistics(tripPoint);
        }

        void PopRecordButtonAnimation()
        {
            if (recordButton.Hidden && PastTripsDetailViewModel == null)
                recordButton.Pop(0.5, 0, 1);
        }

        #endregion

        #region Shared User Interface Logic

        void UpdateCarAnnotationPosition(CLLocationCoordinate2D coordinate)
        {
            if (currentLocationAnnotation != null)
                tripMapView.RemoveAnnotation(currentLocationAnnotation);

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