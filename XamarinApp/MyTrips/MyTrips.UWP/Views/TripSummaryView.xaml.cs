using MyTrips.DataObjects;
using MyTrips.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyTrips.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TripSummaryView : Page
    {
        TripSummaryViewModel viewModel;
        private IList<BasicGeoposition> Locations = new List<BasicGeoposition>();
        public TripSummaryView()
        {
            this.InitializeComponent();
            viewModel = new TripSummaryViewModel();
            viewModel.Trip = getTestTrip();

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


        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {

            base.OnNavigatedTo(e);

            DrawPath();
        }

        private void DrawPath()
        {
            MapPolyline mapPolyLine = new MapPolyline();

            foreach (var trail in this.viewModel.Trip.Trail)
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
            mapStartIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);

            //Draw End Icon
            MapIcon mapEndIcon = new MapIcon();
            mapEndIcon.Location = new Geopoint(Locations.Last());
            mapEndIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);
            mapEndIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/ic_end_point.png"));
            mapEndIcon.ZIndex = 1;
            mapEndIcon.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;
            MyMap.MapElements.Add(mapEndIcon);


        }



        public Trip getTestTrip()
        {
            var trips = MyTrips.DataStore.Mock.Stores.TripStore.GetTrips();
            return trips[4];
        }
    }

}
