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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyTrips.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CurrentTripView : Page, INotifyPropertyChanged
    {
        CurrentTripViewModel viewModel;

        private ImageSource recordButtonImage;

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
            if(PropertyChanged!=null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public CurrentTripView()
        {
            this.InitializeComponent();
            this.viewModel = new CurrentTripViewModel();
            this.MyMap.Loaded += MyMap_Loaded;
            this.DataContext = this;
            recordButtonImage = new BitmapImage(new Uri("ms-appx:///Assets/Login/FBLogo.png", UriKind.Absolute));
            OnPropertyChanged(nameof(RecordButtonImage));
            this.startRecordBtn.Click += StartRecordBtn_Click;
            //// this.startRecordBtn.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/ic_car_blue.jpg")) };

            //this.startRecordBtn.
            ////this.startRecordBtn.
            ////this.stopRecordBtn.Click += StopRecordBtn_Click;
        }

        private void MyMap_Loaded(object sender, RoutedEventArgs e)
        {
            this.MyMap.ZoomLevel = 17;
        }

        protected override  void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
    
            viewModel.StartTrackingTripCommand.Execute(null);
            StartTrackingAsync();
        //    await StartLocationService();

        }

        private async void StartTrackingAsync()
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
                    // Value of 2000 milliseconds (2 seconds) 
                    // isn't a requirement, it is just an example.
                    //_geolocator = new Geolocator { ReportInterval = 2000 };

                    // Subscribe to PositionChanged event to get updated tracking positions
                    //_geolocator.PositionChanged += OnPositionChanged;

                    // Subscribe to StatusChanged event to get updates of location status changes
                    //_geolocator.StatusChanged += OnStatusChanged;
                    Acr.UserDialogs.UserDialogs.Instance.Alert("Waiting for update...", "Geolcoation Enabled", "OK");
                    //StartTrackingButton.IsEnabled = false;
                    //StopTrackingButton.IsEnabled = true;
                    break;

                case GeolocationAccessStatus.Denied:
                    Acr.UserDialogs.UserDialogs.Instance.Alert("Please ensure that geolocation is enabled and permissions are allowed for MyTrips to start a recording.",
                                                "Geolcoation Disabled", "OK");
                    break;

                case GeolocationAccessStatus.Unspecified:
                    Acr.UserDialogs.UserDialogs.Instance.Alert("Unspecified Error...");
                    break;
            }
        }
    

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            //this should stop running only if not recording and should continue in the background
            viewModel.StopTrackingTripCommand.Execute(null);    
        }

        private async void StartRecordBtn_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel == null || viewModel.CurrentPosition == null || viewModel.IsBusy)
                return;

            if (viewModel.IsRecording)
            {

                AddEndMarker(new BasicGeoposition(){ Latitude = viewModel.CurrentPosition.Latitude, Longitude = viewModel.CurrentPosition.Longitude });
                UpdateCarIcon(false);
                await viewModel.StopRecordingTripAsync();
            }
            else
            {
                if (!await viewModel.StartRecordingTripAsync())
                    return;
                AddStartMarker(new BasicGeoposition() { Latitude = viewModel.CurrentPosition.Latitude, Longitude = viewModel.CurrentPosition.Longitude });

                UpdateCarIcon(true);
                UpdateStats();
            }
        }

        private void UpdateCarIcon(bool v)
        {
           // throw new NotImplementedException();
        }

        private void AddStartMarker(BasicGeoposition basicGeoposition)
        {
            MapIcon mapStartIcon = new MapIcon();
            mapStartIcon.Location = new Geopoint(basicGeoposition);
            mapStartIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);
            mapStartIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/ic_start_point.png"));
            mapStartIcon.ZIndex = 1;
            mapStartIcon.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;

            MyMap.MapElements.Add(mapStartIcon);
        }

        private void UpdateStats()
        {
            
        }


        private void ResetTrip()
        {
            this.text_mpg.Text = "0";
            this.text_hours.Text = "0";
            this.text_miles.Text = "0";
            this.text_gallons.Text = "0";
            this.text_cost.Text = "0";
        }
        private void AddEndMarker(BasicGeoposition basicGeoposition)
        {
            MapIcon mapEndIcon = new MapIcon();
            mapEndIcon.Location = new Geopoint(basicGeoposition);
            mapEndIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);
            mapEndIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/ic_end_point.png"));
            mapEndIcon.ZIndex = 1;
            mapEndIcon.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;
            MyMap.MapElements.Add(mapEndIcon);
        }
    }
}
