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
        public CurrentTripViewModel viewModel;
       
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
            this.MyMap.ZoomLevel = 17;
            this.CarIcon = new MapIcon();
            this.mapPolyline = new MapPolyline();
            MyMap.MapElements.Clear();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            viewModel.PropertyChanged += OnPropertyChanged;
            await StartTrackingAsync();
        }

        void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //PropertyChanged(this, new PropertyChangedEventArgs("viewModel"));
            switch (e.PropertyName)
            {
                case nameof(viewModel.CurrentPosition):
                    var basicGeoposition = new BasicGeoposition() { Latitude = viewModel.CurrentPosition.Latitude, Longitude = viewModel.CurrentPosition.Longitude };

                    UpdateMap_PositionChanged(basicGeoposition);
                    UpdateMapView(basicGeoposition);
                    UpdateStats();
                    break;

                case nameof(viewModel.CurrentTrip):
                    ResetTrip();
                    break;

                //    Todo VJ. Fix Databinding issue to directly update the UI. Currently updating manually.
                case nameof(viewModel.Distance):
                case nameof(viewModel.EngineLoad):
                case nameof(viewModel.FuelConsumption):
                case nameof(viewModel.ElapsedTime):
                case nameof(viewModel.DistanceUnits):
                case nameof(viewModel.FuelConsumptionUnits):
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
                // Need to update Map UI before we start saving. So that the entire trip is visible. 
                UpdateMap_PositionChanged(basicGeoposition);

                if (!(await viewModel.StopRecordingTrip()))
                    return;

                // Need to add the end marker only when we are able to stop the trip. 
                AddEndMarker(basicGeoposition);

                recordButtonImage = new BitmapImage(new Uri("ms-appx:///Assets/StartRecord.png", UriKind.Absolute));
                OnPropertyChanged(nameof(RecordButtonImage));
                var recordedTripSummary = viewModel.TripSummary;
                await viewModel.SaveRecordingTripAsync();
                // Launch Trip Summary Page. 
               
                this.Frame.Navigate(typeof(TripSummaryView), recordedTripSummary);
                return;
        }
            else
            {
                if (!(await viewModel.StartRecordingTrip()))
                    return;

                // Update UI to start recording.
                recordButtonImage = new BitmapImage(new Uri("ms-appx:///Assets/StopRecord.png", UriKind.Absolute));
                OnPropertyChanged(nameof(RecordButtonImage));
                // Update the Map with StartMarker, Path
                UpdateMap_PositionChanged(basicGeoposition);
                UpdateStats();
            }
        }

        private async void UpdateMap_PositionChanged(BasicGeoposition basicGeoposition)
        {
            if (viewModel.IsBusy)
                return;
 
            // To update the carIcon first find it and remove it from the MapElements
            await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                // Clear all the map elements. 
                MyMap.MapElements.Clear();

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
                MyMap.MapElements.Add(CarIcon);
            });


            // Add the Start Icon
            AddStartMarker();

            // Add Path if we are recording 
            DrawPath();

        }

        private async void AddStartMarker()
        {
            if (!viewModel.IsRecording || viewModel.CurrentTrip.Points.Count == 0)
                return;

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // First point of the trip will be Start Position. 
                var basicGeoposition = new BasicGeoposition() { Latitude = viewModel.CurrentTrip.Points.First().Latitude, Longitude = viewModel.CurrentTrip.Points.First().Longitude };
                MapIcon mapStartIcon = new MapIcon();
                mapStartIcon.Location = new Geopoint(basicGeoposition);
                mapStartIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);
                mapStartIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/ic_start_point.png"));
                mapStartIcon.ZIndex = 3;
                mapStartIcon.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;
               //   MyMap.Center = mapStartIcon.Location;
                MyMap.MapElements.Add(mapStartIcon);
            });
        }

        private async void DrawPath()
        {
            if (!viewModel.IsRecording || viewModel.CurrentTrip.Points.Count == 0)
                return;

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (MyMap == null)
                    return;
                Locations = new List<BasicGeoposition>(viewModel.CurrentTrip.Points.Select(s => new BasicGeoposition() { Latitude = s.Latitude, Longitude = s.Longitude }));

                mapPolyline.Path = new Geopath(Locations);
                mapPolyline.StrokeColor = Colors.Red;
                mapPolyline.StrokeThickness = 3;
                MyMap.MapElements.Add(mapPolyline);
            });
 
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
                this.text_time.Text = viewModel.ElapsedTime;
                this.text_miles.Text = viewModel.Distance;
                this.text_gallons.Text = viewModel.FuelConsumption;
                this.text_temp.Text = viewModel.EngineLoad;
                this.text_distanceunits.Text = viewModel.DistanceUnits;
                this.text_fuelunits.Text = viewModel.FuelConsumptionUnits;
            });
        }


        private void ResetTrip()
        {
           // MyMap.MapElements.Clear();
            Locations?.Clear();
            Locations = null;
            UpdateStats();
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
