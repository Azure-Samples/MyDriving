using MyTrips.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using MyTrips.DataObjects;
using Windows.UI;
using MvvmHelpers;
using System.Collections.Specialized;
using Windows.UI.Core;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyTrips.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CurrentTripView : Page, INotifyPropertyChanged
    {
        CurrentTripViewModel viewModel;

        ObservableRangeCollection<TripPoint> trailPointList;

        private MapIcon CarIcon;

        private MapPolyline mapPolyline;

        private ImageSource recordButtonImage;

        public IList<BasicGeoposition> Locations { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        public ImageSource RecordButtonImage
        {
            get
            {
                return recordButtonImage;
            }
        }

        //private Geolocator geolocator = null;
        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public CurrentTripView()
        {
            this.InitializeComponent();
            this.viewModel = new CurrentTripViewModel();
            this.Locations = new List<BasicGeoposition>();
            this.MyMap.Loaded += MyMap_Loaded;
            this.DataContext = this;
            recordButtonImage = new BitmapImage(new Uri("ms-appx:///Assets/StartRecord.png", UriKind.Absolute));
            OnPropertyChanged(nameof(RecordButtonImage));
            this.startRecordBtn.Click += StartRecordBtn_Click;
        }

        private void MyMap_Loaded(object sender, RoutedEventArgs e)
        {
            //MyMap.MapElements.Clear();
            this.MyMap.ZoomLevel = 17;
            this.CarIcon = new MapIcon();
            this.mapPolyline = new MapPolyline();
            MyMap.MapElements.Clear();

            MyMap.Center =
               new Geopoint(new BasicGeoposition()
               {
                    //Geopoint for Seattle 
                    Latitude = 47.604,
                   Longitude = -122.329
               });
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            viewModel.PropertyChanged += OnPropertyChanged;
            await StartTrackingAsync();
        }

        void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(viewModel.CurrentPosition):
                    var basicGeoposition = new BasicGeoposition() { Latitude = viewModel.CurrentPosition.Latitude, Longitude = viewModel.CurrentPosition.Longitude };
                    if (viewModel.CurrentTrip.Points.Count > 0)
                    {
                        var item = viewModel.CurrentTrip.Points.Last();
                        UpdateMap(item);
                    }

                    UpdateCarIcon(basicGeoposition);
                    UpdateMapView(basicGeoposition);
                    UpdateStats();
                    break;

                case nameof(viewModel.CurrentTrip):
                    ResetTrip();
                    break;
                case "Distance":
                    UpdateStats();
                    break;
            }
        }

  
        private async Task StartTrackingAsync()
        {
            // Request permission to access location
            var accessStatus = await Geolocator.RequestAccessAsync();

            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    // You should set MovementThreshold for distance-based tracking
                    // or ReportInterval for periodic-based tracking before adding event
                    // handlers. If none is set, a ReportInterval of 1 second is used
                    // as a default and a position will be returned every 1 second.
                    //
                    startRecordBtn.IsEnabled = true;

                   viewModel.StartTrackingTripCommand.Execute(null);

                    break;

                case GeolocationAccessStatus.Denied:
                    Acr.UserDialogs.UserDialogs.Instance.Alert("Please ensure that geolocation is enabled and permissions are allowed for MyTrips to start a recording.",
                                                "Geolcoation Disabled", "OK");
                    startRecordBtn.IsEnabled = false;
                    break;

                case GeolocationAccessStatus.Unspecified:
                    Acr.UserDialogs.UserDialogs.Instance.Alert("Unspecified Error...", "Geolcoation Disabled", "OK");
                    startRecordBtn.IsEnabled = false;
                    break;
            }
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            //Ideally, we should stop tracking only if we aren't recording
            viewModel.StopTrackingTripCommand.Execute(null);
            Locations?.Clear();
            Locations = null;
            MyMap.MapElements.Clear();
            this.MyMap.Loaded -= MyMap_Loaded;
            this.startRecordBtn.Click -= StartRecordBtn_Click;
            viewModel.PropertyChanged -= OnPropertyChanged;
        }

        private async void StartRecordBtn_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel == null || viewModel.CurrentPosition == null || viewModel.IsBusy)
                return;

            var basicGeoposition = new BasicGeoposition() { Latitude = viewModel.CurrentPosition.Latitude, Longitude = viewModel.CurrentPosition.Longitude };

            if (viewModel.IsRecording)
            {
                AddEndMarker(basicGeoposition);
                recordButtonImage = new BitmapImage(new Uri("ms-appx:///Assets/StartRecord.png", UriKind.Absolute));
                OnPropertyChanged(nameof(RecordButtonImage));
                
                //UpdateCarIcon(basicGeoposition);
                await viewModel.StopRecordingTripAsync();
                // Launch Trip Summary Page. 
                this.Frame.Navigate(typeof(TripSummaryView), viewModel.CurrentTrip);
                return;
           }
            else
            {
                if (!await viewModel.StartRecordingTripAsync())
                    return;
                recordButtonImage = new BitmapImage(new Uri("ms-appx:///Assets/StopRecord.png", UriKind.Absolute));
                OnPropertyChanged(nameof(RecordButtonImage));
                AddStartMarker(basicGeoposition);

                UpdateCarIcon(basicGeoposition);
                UpdateStats();
            }
        }

        private async void UpdateCarIcon(BasicGeoposition basicGeoposition)
        {
            if (viewModel.IsBusy)
                return;

            // To update the carIcon first find it and remove it from the MapElements
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (MyMap.MapElements.Count > 0)
                {
                    var index = MyMap.MapElements.IndexOf(CarIcon);
                    if (index > 0)
                        MyMap.MapElements.RemoveAt(index);
                }
                CarIcon = new MapIcon();
                CarIcon.Location = new Geopoint(basicGeoposition);
                CarIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);

                if (viewModel.IsRecording)
                    CarIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/ic_car_red.png"));
                else
                    CarIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/ic_car_blue.png"));

                CarIcon.ZIndex = 4;
                CarIcon.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;

                MyMap.Center = CarIcon.Location;
                //MyMap.MapElements.Clear();
                MyMap.MapElements.Add(CarIcon);
            });
         
        }

        private async void AddStartMarker(BasicGeoposition basicGeoposition)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                MapIcon mapStartIcon = new MapIcon();
                mapStartIcon.Location = new Geopoint(basicGeoposition);
                mapStartIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);
                mapStartIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/ic_start_point.png"));
                mapStartIcon.ZIndex = 3;
                mapStartIcon.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;
                MyMap.Center = mapStartIcon.Location;
                MyMap.MapElements.Add(mapStartIcon);
            });
        }

        async void UpdateMap(TripPoint trail, bool updateCamera = true)
        {
            var basicGeoposition = new BasicGeoposition();

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                if (MyMap == null)
                    return;
                //Get trail position or current potion to move car

            
                if (viewModel?.CurrentPosition != null)
                {
                    basicGeoposition.Latitude = viewModel.CurrentPosition.Latitude;
                    basicGeoposition.Longitude = viewModel.CurrentPosition.Longitude;
                }

                UpdateCarIcon(basicGeoposition);

                // Remove the Polyline 
                var index = MyMap.MapElements.IndexOf(mapPolyline);
                if (index > 0)
                    MyMap.MapElements.RemoveAt(index);

                if (Locations == null)
                {
                    Locations = new List<BasicGeoposition>(viewModel.CurrentTrip.Points.Select(s => new BasicGeoposition() { Latitude = s.Latitude, Longitude = s.Longitude }));
                }
                else if (trail != null)
                {
                    basicGeoposition = new BasicGeoposition() { Latitude = trail.Latitude, Longitude = trail.Longitude };
                    Locations.Add(basicGeoposition);
                }

                mapPolyline.Path = new Geopath(Locations);
                if (viewModel.IsRecording)
                    mapPolyline.StrokeColor = Colors.Red;
                else
                    mapPolyline.StrokeColor = Colors.Blue;
                mapPolyline.StrokeThickness = 3;
                MyMap.MapElements.Add(mapPolyline);

                // Moves the camera to make the trail location as the center of the view. 
            });
             basicGeoposition = new BasicGeoposition() { Latitude = trail.Latitude, Longitude = trail.Longitude };

            if (updateCamera && !viewModel.IsBusy)
                await MyMap.TrySetViewAsync(new Geopoint(basicGeoposition));
        }

        private async void UpdateMapView(BasicGeoposition basicGeoposition)
        {
            var geoPoint = new Geopoint(basicGeoposition);
            if (!viewModel.IsBusy)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    this.MyMap.Center = geoPoint;
                });
                await this.MyMap.TrySetViewAsync(geoPoint);
            }
        }

        private async void UpdateStats()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.text_mpg.Text = "0";
                this.text_hours.Text = viewModel.ElapsedTime;
                this.text_miles.Text = viewModel.CurrentTrip.TotalDistanceNoUnits;
                this.text_gallons.Text = "0";
                this.text_cost.Text = "$0.00";
            });
        }


        private void ResetTrip()
        {
            trailPointList = viewModel.CurrentTrip.Points as ObservableRangeCollection<TripPoint>;
            trailPointList.CollectionChanged += OnTrailUpdated;
           // MyMap.MapElements.Clear();
            Locations?.Clear();
            Locations = null;
            SetupMap();
            UpdateStats();
        }

        private void SetupMap()
        {
            TripPoint start = null;
            if (viewModel.CurrentTrip.Points.Count == 0)
                return;

            start = viewModel.CurrentTrip.Points[0];
            UpdateMap(start, false);
            AddStartMarker(new BasicGeoposition() { Latitude = start.Latitude, Longitude = start.Longitude });
        }

        private void OnTrailUpdated(object sender, NotifyCollectionChangedEventArgs e)
        {
            var item = viewModel.CurrentTrip.Points[viewModel.CurrentTrip.Points.Count - 1];
            UpdateMap(item);
        }

        private async void AddEndMarker(BasicGeoposition basicGeoposition)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                MapIcon mapEndIcon = new MapIcon();
                mapEndIcon.Location = new Geopoint(basicGeoposition);
                mapEndIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);
                mapEndIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/ic_end_point.png"));
                mapEndIcon.ZIndex = 3;
                mapEndIcon.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;
                MyMap.MapElements.Add(mapEndIcon);
            });
        }
    }
}
