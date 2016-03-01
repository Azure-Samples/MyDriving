using System;
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

        RandomAccessStreamReference mapIconStreamReference;

        public PastTripMapView()
        {
            this.InitializeComponent();
            this.ViewModel = new PastTripsDetailViewModel();
            this.Locations = new List<BasicGeoposition>();
        }

        PastTripsDetailViewModel ViewModel;
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var trip = e.Parameter as Trip;
            base.OnNavigatedTo(e);
            this.MyMap.Loaded += MyMap_Loaded;
            this.ViewModel.Trip = trip;
            mapIconStreamReference = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/ic_start_point.png"));
            DrawPath();
        }

        private void MyMap_Loaded(object sender, RoutedEventArgs e)
        {
            this.MyMap.ZoomLevel = 17;
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
            if ( Locations.Count > 0)
                MyMap.Center = new Geopoint(Locations.First());
            
            
            MyMap.MapElements.Add(mapPolyLine);

            MapIcon mapIcon = new MapIcon();
            mapIcon.Location = MyMap.Center;
            mapIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
            mapIcon.Image = mapIconStreamReference;
            mapIcon.ZIndex = 0;
            mapIcon.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;
          
            MyMap.MapElements.Add(mapIcon);
            
        }

        private void positionSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {

        }
    }
}
