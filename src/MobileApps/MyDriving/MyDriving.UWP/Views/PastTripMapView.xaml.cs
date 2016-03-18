// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;
using MyDriving.DataObjects;
using MyDriving.ViewModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyDriving.UWP.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PastTripMapView : Page
    {
        readonly PastTripsDetailViewModel _viewModel;

        public Trip SelectedTrip;

        public PastTripMapView()
        {
            InitializeComponent();
            _viewModel = new PastTripsDetailViewModel();
            Locations = new List<BasicGeoposition>();
            DataContext = this;
        }

        public IList<BasicGeoposition> Locations { get; set; }

        public List<TripPoint> TripPoints { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var trip = e.Parameter as Trip;
            base.OnNavigatedTo(e);
            MyMap.Loaded += MyMap_Loaded;
            MyMap.MapElements.Clear();
            _viewModel.Trip = trip;
            DrawPath();

            // Currently Points are all jumbled. We need to investigate why this is happening.
            // As a workaround I am sorting the points based on timestamp.  
            TripPoints = _viewModel.Trip.Points.OrderBy(p => p.RecordedTimeStamp).ToList();

            if (TripPoints.Any())
            {
                _viewModel.CurrentPosition = TripPoints[0];
                UpdateStats();
            }
            // Enable the back button navigation
            SystemNavigationManager systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            systemNavigationManager.BackRequested += SystemNavigationManager_BackRequested; 
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            SystemNavigationManager systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            systemNavigationManager.BackRequested -= SystemNavigationManager_BackRequested;
        }

        private void SystemNavigationManager_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = TryGoBack();
            }
        }

        private bool TryGoBack()
        {
            bool navigated = false;
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
                navigated = true;
            }
            return navigated;
        }

        private void MyMap_Loaded(object sender, RoutedEventArgs e)
        {
            MyMap.ZoomLevel = 16;
            if (_viewModel.Trip.Points.Count > 0)
                positionSlider.Maximum = TripPoints.Count - 1;
            else
                positionSlider.Maximum = 0;

            positionSlider.Minimum = 0;
            positionSlider.IsThumbToolTipEnabled = false;

            text_starttime.Text = _viewModel.Trip.StartTimeDisplay;
            text_endtime.Text = _viewModel.Trip.EndTimeDisplay;
        }

        private async void DrawPath()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                MapPolyline mapPolyLine = new MapPolyline();

                Locations =
                    TripPoints.Select(s => new BasicGeoposition() {Latitude = s.Latitude, Longitude = s.Longitude})
                        .ToList();

                mapPolyLine.Path = new Geopath(Locations);

                mapPolyLine.ZIndex = 1;
                mapPolyLine.Visible = true;
                mapPolyLine.StrokeColor = Colors.Red;
                mapPolyLine.StrokeThickness = 4;

                // Starting off with the first point as center
                if (Locations.Count > 0)
                    MyMap.Center = new Geopoint(Locations.First());

                MyMap.MapElements.Add(mapPolyLine);

                // Draw Start Icon
                MapIcon mapStartIcon = new MapIcon
                {
                    Location = new Geopoint(Locations.First()),
                    NormalizedAnchorPoint = new Point(0.5, 0.5),
                    Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/ic_start_point.png")),
                    ZIndex = 1,
                    CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible
                };

                MyMap.MapElements.Add(mapStartIcon);

                //Draw End Icon
                MapIcon mapEndIcon = new MapIcon
                {
                    Location = new Geopoint(Locations.Last()),
                    NormalizedAnchorPoint = new Point(0.5, 0.5),
                    Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/ic_end_point.png")),
                    ZIndex = 1,
                    CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible
                };
                MyMap.MapElements.Add(mapEndIcon);

                // Draw the Car 
                DrawCarOnMap(this.Locations.First());
            });
        }

        private void DrawPoiOnMap()
        {
            // Foreach POI point. Put it on Maps. 
            MapIcon mapEndIcon = new MapIcon();
            mapEndIcon.Location = new Geopoint(this.Locations.First());
            mapEndIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);
            mapEndIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/ic_end_point.png"));
            mapEndIcon.ZIndex = 1;
            mapEndIcon.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;
            MyMap.MapElements.Add(mapEndIcon);

        }

        private void DrawCarOnMap(BasicGeoposition basicGeoposition)
        {
            MapIcon mapCarIcon = new MapIcon
            {
                Location = new Geopoint(basicGeoposition),
                NormalizedAnchorPoint = new Point(0.5, 0.5),
                Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/ic_car_red.png")),
                ZIndex = 2,
                CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible
            };


            MyMap.MapElements.Add(mapCarIcon);
            MyMap.Center = mapCarIcon.Location;
        }

        private async void positionSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            _viewModel.CurrentPosition = TripPoints[(int) e.NewValue];

            var basicGeoposition = Locations[(int) e.NewValue];
            // Currently removing the Car from Map which is the last item added. 
            MyMap.MapElements.RemoveAt(MyMap.MapElements.Count - 1);
            DrawCarOnMap(basicGeoposition);
            await MyMap.TrySetViewAsync(new Geopoint(basicGeoposition));
            UpdateStats();
        }

        private async void UpdateStats()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // TODO: Need to fix data binding and remove this code. 
                text_time.Text = _viewModel.ElapsedTime;
                text_distance.Text = _viewModel.Distance;
                text_fuel.Text = _viewModel.FuelConsumption;
                text_fuelunits.Text = _viewModel.FuelConsumptionUnits;
                text_speed.Text = _viewModel.Speed;
                text_speedunits.Text = _viewModel.SpeedUnits;
                text_distanceunits.Text = _viewModel.DistanceUnits;
            });
        }
    }
}
