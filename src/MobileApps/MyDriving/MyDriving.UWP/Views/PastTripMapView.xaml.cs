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
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;
using MyDriving.DataObjects;
using MyDriving.ViewModel;
using MyDriving.Utils;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyDriving.UWP.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PastTripMapView
    {
        readonly PastTripsDetailViewModel viewModel;

        public Trip SelectedTrip;

        public PastTripMapView()
        {
            InitializeComponent();
            viewModel = new PastTripsDetailViewModel();
            Locations = new List<BasicGeoposition>();
            DataContext = this;

            if (Logger.BingMapsAPIKey != "____BingMapsAPIKey____")
            {
                MyMap.MapServiceToken = Logger.BingMapsAPIKey;
            }
        }

        public IList<BasicGeoposition> Locations { get; set; }

        public List<TripPoint> TripPoints { get; set; }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var trip = e.Parameter as Trip;
            base.OnNavigatedTo(e);
            viewModel.Trip = trip;
            App.SetTitle("PAST TRIPS");
            MyMap.Loaded += MyMap_Loaded;
            MyMap.MapElements.Clear();
            var success = await viewModel.ExecuteLoadTripCommandAsync(trip.Id);
            if(!success)
            {
                Frame.GoBack();
                return;
            }
            DrawPath();

            foreach (var poi in viewModel.POIs)
                DrawPoiOnMap(poi);

            // Currently Points are all jumbled. We need to investigate why this is happening.
            // As a workaround I am sorting the points based on timestamp.  
            TripPoints = viewModel.Trip.Points.OrderBy(p => p.RecordedTimeStamp).ToList();

            if (TripPoints.Any())
            {
                viewModel.CurrentPosition = TripPoints[0];
                UpdateStats();
            }


            if (mapLoaded)
                InitialSetup();
            


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

        bool initialized;
        void InitialSetup()
        {
            if (initialized)
                return;

            initialized = true;
            MyMap.ZoomLevel = 16;
            if (viewModel.Trip.Points.Count > 0)
                PositionSlider.Maximum = TripPoints.Count - 1;
            else
                PositionSlider.Maximum = 0;

            PositionSlider.Minimum = 0;
            PositionSlider.IsThumbToolTipEnabled = false;

            TextStarttime.Text = viewModel.Trip.StartTimeDisplay;
            TextEndtime.Text = viewModel.Trip.EndTimeDisplay;
        }

        bool mapLoaded;
        private void MyMap_Loaded(object sender, RoutedEventArgs e)
        {
            mapLoaded = true;
            if (!initialized && TripPoints != null)
                InitialSetup();
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
                mapPolyLine.StrokeColor = new Color { A = 255, R = 0, G = 94, B = 147 };
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
                    Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/A100.png")),
                    ZIndex = 1,
                    CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible
                };

                MyMap.MapElements.Add(mapStartIcon);

                //Draw End Icon
                MapIcon mapEndIcon = new MapIcon
                {
                    Location = new Geopoint(Locations.Last()),
                    NormalizedAnchorPoint = new Point(0.5, 0.5),
                    Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/B100.png")),
                    ZIndex = 1,
                    CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible
                };
                MyMap.MapElements.Add(mapEndIcon);

                // Draw the Car 
                DrawCarOnMap(Locations.First());
            });
        }

        private void DrawPoiOnMap(POI poi)
        {
            // Foreach POI point. Put it on Maps. 
            var poiIcon = new MapIcon
            {
                Location = new Geopoint(new BasicGeoposition { Latitude = poi.Latitude, Longitude = poi.Longitude }),
                NormalizedAnchorPoint = new Point(0.5, 0.5),
                Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/POI.png")),
                ZIndex = 2,
                CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible
            };
            MyMap.MapElements.Add(poiIcon);
        }

        private void DrawCarOnMap(BasicGeoposition basicGeoposition)
        {
            MapIcon carIcon = null;
            // Find if there is a MapIcon with title Car
            if (MyMap.MapElements != null)
            {
                var mapIcons = MyMap.MapElements.OfType<MapIcon>().ToList();
                foreach (var item in mapIcons)
                {
                    if (item.Title == "Car")
                        carIcon = item;
                }
            }
            
            if (carIcon == null)
            {
                // Car Icon not found creating it at the position and adding to maps
                carIcon = new MapIcon
            {
                Location = new Geopoint(basicGeoposition),
                NormalizedAnchorPoint = new Point(0.5, 0.5),
                    Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/BlueCar.png")),
                ZIndex = 2,
                    CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible,
                    Title = "Car"
            };
                MyMap.MapElements.Add(carIcon);
            }
            else
            {
                carIcon.Location = new Geopoint(basicGeoposition);
            }
            MyMap.Center = carIcon.Location;
        }

        private async void positionSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            viewModel.CurrentPosition = TripPoints[(int) e.NewValue];

            var basicGeoposition = Locations[(int) e.NewValue];
            DrawCarOnMap(basicGeoposition);
            await MyMap.TrySetViewAsync(new Geopoint(basicGeoposition));
            UpdateStats();
        }

        private async void UpdateStats()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // TODO: Need to fix data binding and remove this code. 
                TextTime.Text = viewModel.ElapsedTime;
                TextDistance.Text = viewModel.Distance;
                TextFuel.Text = viewModel.FuelConsumption;
                TextFuelunits.Text = viewModel.FuelConsumptionUnits;
                TextSpeed.Text = viewModel.Speed;
                TextSpeedunits.Text = viewModel.SpeedUnits;
                TextDistanceunits.Text = viewModel.DistanceUnits;
            });
        }
    }
}
