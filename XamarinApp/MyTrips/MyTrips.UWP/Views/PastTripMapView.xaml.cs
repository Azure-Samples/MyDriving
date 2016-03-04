﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MyTrips.DataObjects;
using MyTrips.ViewModel;
using Windows.UI.Xaml.Controls.Maps;
using Windows.Devices.Geolocation;
using Windows.UI;
using Windows.Storage.Streams;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyTrips.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PastTripMapView : Page
    {

        public IList<BasicGeoposition> Locations { get; set; }

      
        public PastTripMapView()
        {
            this.InitializeComponent();
            this.ViewModel = new PastTripsDetailViewModel();
            this.Locations = new List<BasicGeoposition>();
        }

        PastTripsDetailViewModel ViewModel;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var trip = e.Parameter as Trip;
            base.OnNavigatedTo(e);
            this.MyMap.Loaded += MyMap_Loaded;
            this.ViewModel.Trip = trip;
            DrawPath();
        }

        private void MyMap_Loaded(object sender, RoutedEventArgs e)
        {
            this.MyMap.ZoomLevel = 17;
            this.positionSlider.Maximum = this.ViewModel.Trip.Trail.Count - 1;
            this.positionSlider.Minimum = 0;
            this.positionSlider.IsThumbToolTipEnabled = false;
        }

        private void DrawPath()
        {
            MapPolyline mapPolyLine = new MapPolyline();

            foreach (var trail in this.ViewModel.Trip.Trail)
            {
                var basicGeoPosion = new BasicGeoposition() { Latitude = trail.Latitude, Longitude = trail.Longitude };
                Locations.Add(basicGeoPosion);
            }
            mapPolyLine.Path = new Geopath(Locations);

            mapPolyLine.ZIndex = 1;
            mapPolyLine.Visible = true;
            mapPolyLine.StrokeColor = Colors.Red;
            mapPolyLine.StrokeThickness = 3;

            // Starting off with the first point as center
            if (Locations.Count > 0)
                MyMap.Center = new Geopoint(Locations.First());


            MyMap.MapElements.Add(mapPolyLine);

            // Draw Start Icon
            MapIcon mapStartIcon = new MapIcon();
            mapStartIcon.Location = new Geopoint(Locations.First());
            mapStartIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);
            mapStartIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/ic_start_point.png"));
            mapStartIcon.ZIndex = 1;
            mapStartIcon.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;

            MyMap.MapElements.Add(mapStartIcon);
     
            //Draw End Icon
            MapIcon mapEndIcon = new MapIcon();
            mapEndIcon.Location = new Geopoint(Locations.Last());
            mapEndIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);
            mapEndIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/ic_end_point.png"));
            mapEndIcon.ZIndex = 1;
            mapEndIcon.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;
            MyMap.MapElements.Add(mapEndIcon);

            // Draw the Car 
            DrawCarOnMap(Locations.First());

        }

        private void DrawCarOnMap(BasicGeoposition basicGeoposition)
        {
            MapIcon mapCarIcon = new MapIcon();
            mapCarIcon.Location = new Geopoint(basicGeoposition);
            mapCarIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);

            mapCarIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/ic_car_red.png"));
            mapCarIcon.ZIndex = 2;
            mapCarIcon.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;

            MyMap.MapElements.Add(mapCarIcon);
            MyMap.Center = mapCarIcon.Location;

        }

        private async void positionSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            var location = this.ViewModel.Trip.Trail[(int)e.NewValue];
            var basicGeoposition = new BasicGeoposition { Latitude = location.Latitude, Longitude = location.Longitude };
            // Currently removing the Car from Map which is the last item added. 
            MyMap.MapElements.RemoveAt(MyMap.MapElements.Count - 1);
            DrawCarOnMap(basicGeoposition);
            await MyMap.TrySetViewAsync(new Geopoint(basicGeoposition));
        }
    }
}
