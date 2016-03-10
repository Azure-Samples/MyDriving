using MyTrips.DataObjects;
using MyTrips.ViewModel;
using System;
using System.Collections.Generic;
using Windows.Devices.Geolocation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Navigation;
using MyTrips.UWP;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyTrips.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TripSummaryView : Page
    {
        TripSummaryViewModel viewModel;
        private List<BasicGeoposition> Locations = new List<BasicGeoposition>();
        public TripSummaryView()
        {
            this.InitializeComponent();
            viewModel = new TripSummaryViewModel();

            DataContext = viewModel;

            TotalDistanceTab.Title1 = "Total";
            TotalDistanceTab.Title2 = "DISTANCE";

            TotalTimeTab.Title1 = "Total";
            TotalTimeTab.Title2 = "TIME";

            AvgSpeedTab.Title1 = "Avg";
            AvgSpeedTab.Title2 = "SPEED";

            HardBreaksTab.Title1 = "Hard";
            HardBreaksTab.Title2 = "BREAKS";

            TipsTab.Title1 = "TIPS";
        }


        /*protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //viewModel.Trip = e.Parameter as Trip;
            //DrawPath();
        }

        private void DrawPath()
        {
            MapPolyline mapPolyLine = new MapPolyline();
            
            if (viewModel..Points.Count == 0)
                return;

            foreach (var trail in this.viewModel.Trip.Points)
            {
                var basicGeoPosion = new BasicGeoposition() { Latitude = trail.Latitude, Longitude = trail.Longitude };
                Locations.Add(basicGeoPosion);
            }
            mapPolyLine.Path = new Geopath(Locations);

            mapPolyLine.ZIndex = 1;
            mapPolyLine.Visible = true;
            mapPolyLine.StrokeColor = Colors.Red;
            mapPolyLine.StrokeThickness = 3;

            // compute center
            if (Locations.Count > 0)
            {
                double north, east, south, west;

                north = south = Locations[0].Latitude;
                west = east = Locations[0].Longitude;

                foreach (var p in Locations)
                {
                    if (north < p.Latitude) north = p.Latitude;
                    if (west > p.Longitude) west = p.Longitude;
                    if (south > p.Latitude) south = p.Latitude;
                    if (east < p.Longitude) east = p.Longitude;
                }


                BasicGeoposition pos = new BasicGeoposition();
                pos.Latitude = (north + south) / 2;
                pos.Longitude = (east + west) / 2;
                var center = new Geopoint(pos);
                MyMap.Center = center;

                //find zoom
                double buffer = 2;
                double zoom1, zoom2;

                if (east != west && north != south)
                {
                    //best zoom level based on map width
                    zoom1 = Math.Log(360.0 / 256.0 * (MyMap.Width - 2 * buffer) / (east - west)) / Math.Log(2);
                    //best zoom level based on map height
                    zoom2 = Math.Log(180.0 / 256.0 * (MyMap.Height - 2 * buffer) / (north - south)) / Math.Log(2);
                }
                else
                {
                    zoom1 = zoom2 = 15;
                }

                //use the most zoomed out of the two zoom levels
                double zoomLevel = (zoom1 < zoom2) ? zoom1 : zoom2;
                MyMap.ZoomLevel = zoomLevel;
            }

            MyMap.MapElements.Add(mapPolyLine);
        }*/
    }

}
