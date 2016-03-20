// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using MyDriving.ViewModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyDriving.UWP.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CurrentTripView : INotifyPropertyChanged
    {
        private MapIcon carIcon;

        private MapPolyline mapPolyline;

        private ImageSource recordButtonImage;

        private ExtendedExecutionSession session;
        public CurrentTripViewModel ViewModel;

        public CurrentTripView()
        {
            InitializeComponent();
            ViewModel = new CurrentTripViewModel();
            Locations = new List<BasicGeoposition>();
            MyMap.Loaded += MyMap_Loaded;
            DataContext = this;
            recordButtonImage = new BitmapImage(new Uri("ms-appx:///Assets/StartRecord.png", UriKind.Absolute));
            OnPropertyChanged(nameof(RecordButtonImage));
            StartRecordBtn.Click += StartRecordBtn_Click;
        }

        public IList<BasicGeoposition> Locations { get; set; }

        public ImageSource RecordButtonImage => recordButtonImage;


        public event PropertyChangedEventHandler PropertyChanged;

        //private Geolocator geolocator = null;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        private void MyMap_Loaded(object sender, RoutedEventArgs e)
        {
            MyMap.ZoomLevel = 16;
            carIcon = new MapIcon();
            mapPolyline = new MapPolyline();
            MyMap.MapElements.Clear();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.PropertyChanged += OnPropertyChanged;
            await StartTrackingAsync();
            UpdateStats();
            SystemNavigationManager systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            systemNavigationManager.BackRequested += SystemNavigationManager_BackRequested;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            //Ideally, we should stop tracking only if we aren't recording
            ViewModel.StopTrackingTripCommand.Execute(null);
            Locations?.Clear();
            Locations = null;
            MyMap.MapElements.Clear();
            MyMap.Loaded -= MyMap_Loaded;
            StartRecordBtn.Click -= StartRecordBtn_Click;
            ViewModel.PropertyChanged -= OnPropertyChanged;
            SystemNavigationManager systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            systemNavigationManager.BackRequested -= SystemNavigationManager_BackRequested;
            ClearExtendedExecution();
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

        void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //PropertyChanged(this, new PropertyChangedEventArgs("viewModel"));
            switch (e.PropertyName)
            {
                case nameof(ViewModel.CurrentPosition):
                    var basicGeoposition = new BasicGeoposition()
                    {
                        Latitude = ViewModel.CurrentPosition.Latitude,
                        Longitude = ViewModel.CurrentPosition.Longitude
                    };

                    UpdateMap_PositionChanged(basicGeoposition);
                    UpdateMapView(basicGeoposition);
                    UpdateStats();
                    break;

                case nameof(ViewModel.CurrentTrip):
                    ResetTrip();
                    break;

                //    Todo VJ. Fix Databinding issue to directly update the UI. Currently updating manually.
                case nameof(ViewModel.Distance):
                case nameof(ViewModel.EngineLoad):
                case nameof(ViewModel.FuelConsumption):
                case nameof(ViewModel.ElapsedTime):
                case nameof(ViewModel.DistanceUnits):
                case nameof(ViewModel.FuelConsumptionUnits):
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
                    // Need to Get the position to Get the map to focus on current position. 
                    /* var position = await ViewModel.Geolocator.GetPositionAsync();
                    var basicPosition = new BasicGeoposition()
                    {
                        Latitude = position.Latitude,
                        Longitude = position.Longitude
                    };
                    UpdateMap_PositionChanged(basicPosition);
                    */
                    StartRecordBtn.IsEnabled = true;
                    await BeginExtendedExecution();
                    break;

                case GeolocationAccessStatus.Denied:
                    Acr.UserDialogs.UserDialogs.Instance.Alert(
                        "Please ensure that geolocation is enabled and permissions are allowed for MyDriving to start a recording.",
                        "Geolocation Disabled", "OK");
                    StartRecordBtn.IsEnabled = false;
                    break;

                case GeolocationAccessStatus.Unspecified:
                    Acr.UserDialogs.UserDialogs.Instance.Alert("Unspecified Error...", "Geolocation Disabled", "OK");
                    StartRecordBtn.IsEnabled = false;
                    break;
            }
        }


        private async Task BeginExtendedExecution()
        {
            if (ViewModel == null)
                return;

            ClearExtendedExecution();

            var newSession = new ExtendedExecutionSession
            {
                Reason = ExtendedExecutionReason.LocationTracking,
                Description = "Tracking your location"
            };
            newSession.Revoked += SessionRevoked;
            ExtendedExecutionResult result = await newSession.RequestExtensionAsync();
            switch (result)
            {
                case ExtendedExecutionResult.Allowed:
                    //Acr.UserDialogs.UserDialogs.Instance.InfoToast("Extended execution allowed.",
                    //                      "Extended Execution", 4000);

                    session = newSession;
                    ViewModel.Geolocator.AllowsBackgroundUpdates = true;
                    ViewModel.StartTrackingTripCommand.Execute(null);

                    break;

                default:
                    Acr.UserDialogs.UserDialogs.Instance.Alert("Unable to execute app in the background.",
                        "Background execution denied.", "OK");

                    newSession.Dispose();
                    break;
            }
        }

        private void ClearExtendedExecution()
        {
            if (session != null)
            {
                session.Revoked -= SessionRevoked;
                session.Dispose();
                session = null;
            }
        }

        private async void SessionRevoked(object sender, ExtendedExecutionRevokedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                switch (args.Reason)
                {
                    case ExtendedExecutionRevokedReason.Resumed:
                        //Acr.UserDialogs.UserDialogs.Instance.InfoToast("Extended execution revoked due to returning to foreground.",
                        //                    "App Resumed", 4000);
                        break;

                    case ExtendedExecutionRevokedReason.SystemPolicy:
                        Acr.UserDialogs.UserDialogs.Instance.Alert("Extended execution revoked due to system policy.",
                            "Background Execution revoked.", "OK");
                        break;
                }
                // Once Resumed we need to start the extended execution again.
                await BeginExtendedExecution();
            });
        }


        private async void StartRecordBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel?.CurrentPosition == null || ViewModel.IsBusy)
                return;

            var basicGeoposition = new BasicGeoposition()
            {
                Latitude = ViewModel.CurrentPosition.Latitude,
                Longitude = ViewModel.CurrentPosition.Longitude
            };

            if (ViewModel.IsRecording)
            {
                // Need to update Map UI before we start saving. So that the entire trip is visible. 
                UpdateMap_PositionChanged(basicGeoposition);

                if (!await ViewModel.StopRecordingTrip())
                    return;

                // Need to add the end marker only when we are able to stop the trip. 
                AddEndMarker(basicGeoposition);

                recordButtonImage = new BitmapImage(new Uri("ms-appx:///Assets/StartRecord.png", UriKind.Absolute));
                OnPropertyChanged(nameof(RecordButtonImage));
                var recordedTripSummary = ViewModel.TripSummary;
                await ViewModel.SaveRecordingTripAsync();
                // Launch Trip Summary Page. 

                Frame.Navigate(typeof (TripSummaryView), recordedTripSummary);
            }
            else
            {
                if (!await ViewModel.StartRecordingTrip())
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
            if (ViewModel.IsBusy)
                return;

            // To update the carIcon first find it and remove it from the MapElements
            await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                // Clear all the map elements. 
                MyMap.MapElements.Clear();

                carIcon = new MapIcon
                {
                    Location = new Geopoint(basicGeoposition),
                    NormalizedAnchorPoint = new Point(0.5, 0.5)
                };

                if (ViewModel.IsRecording)
                    carIcon.Image =
                        RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/ic_car_red.png"));
                else
                    carIcon.Image =
                        RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/ic_car_blue.png"));

                carIcon.ZIndex = 4;
                carIcon.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;
                MyMap.Center = carIcon.Location;
                MyMap.MapElements.Add(carIcon);
            });


            // Add the Start Icon
            AddStartMarker();

            // Add Path if we are recording 
            DrawPath();
        }

        private async void AddStartMarker()
        {
            if (!ViewModel.IsRecording || ViewModel.CurrentTrip.Points.Count == 0)
                return;

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // First point of the trip will be Start Position. 
                var basicGeoposition = new BasicGeoposition()
                {
                    Latitude = ViewModel.CurrentTrip.Points.First().Latitude,
                    Longitude = ViewModel.CurrentTrip.Points.First().Longitude
                };
                MapIcon mapStartIcon = new MapIcon
                {
                    Location = new Geopoint(basicGeoposition),
                    NormalizedAnchorPoint = new Point(0.5, 0.5),
                    Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/ic_start_point.png")),
                    ZIndex = 3,
                    CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible
                };
                //   MyMap.Center = mapStartIcon.Location;
                MyMap.MapElements.Add(mapStartIcon);
            });
        }

        private async void DrawPath()
        {
            if (!ViewModel.IsRecording || ViewModel.CurrentTrip.Points.Count == 0)
                return;

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (MyMap == null)
                    return;
                Locations =
                    new List<BasicGeoposition>(
                        ViewModel.CurrentTrip.Points.Select(
                            s => new BasicGeoposition() {Latitude = s.Latitude, Longitude = s.Longitude}));

                mapPolyline.Path = new Geopath(Locations);
                mapPolyline.StrokeColor = Colors.Red;
                mapPolyline.StrokeThickness = 3;
                MyMap.MapElements.Add(mapPolyline);
            });
        }

        private async void UpdateMapView(BasicGeoposition basicGeoposition)
        {
            var geoPoint = new Geopoint(basicGeoposition);
            if (!ViewModel.IsBusy)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { MyMap.Center = geoPoint; });
                await MyMap.TrySetViewAsync(geoPoint);
            }
        }

        private async void UpdateStats()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                TextFuel.Text = ViewModel.FuelConsumption;
                TextFuelunits.Text = ViewModel.FuelConsumptionUnits;
                TextDistance.Text = ViewModel.Distance;
                TextDistanceunits.Text = ViewModel.DistanceUnits;
                TextTime.Text = ViewModel.ElapsedTime;
                TextEngineload.Text = ViewModel.EngineLoad;
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
                MapIcon mapEndIcon = new MapIcon
                {
                    Location = new Geopoint(basicGeoposition),
                    NormalizedAnchorPoint = new Point(0.5, 0.5),
                    Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/ic_end_point.png")),
                    ZIndex = 3,
                    CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible
                };
                MyMap.MapElements.Add(mapEndIcon);
            });
        }
    }
}