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
using MyDriving.Utils;
using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth.Rfcomm;

namespace MyDriving.UWP.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CurrentTripView : INotifyPropertyChanged
    {
        private bool recordButtonIsBusy = false;
        private ImageSource recordButtonImage;

        private ExtendedExecutionSession session;
        public CurrentTripViewModel ViewModel;

        public CurrentTripView()
        {
            InitializeComponent();
            ViewModel = new CurrentTripViewModel();
            Locations = new List<BasicGeoposition>();

            if (Logger.BingMapsAPIKey != "____BingMapsAPIKey____")
            {
                MyMap.MapServiceToken = Logger.BingMapsAPIKey;
            }

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
            MyMap.MapElements.Clear();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            App.SetTitle("CURRENT TRIP");
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
            if (Frame.CanGoBack && !ViewModel.IsRecording)
            {
                Frame.GoBack();
                navigated = true;
            }
            return navigated;
        }

        void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ViewModel.CurrentPosition):
                    var basicGeoposition = new BasicGeoposition()
                    {
                        Latitude = ViewModel.CurrentPosition.Latitude,
                        Longitude = ViewModel.CurrentPosition.Longitude
                    };

                    UpdateMap_PositionChanged(basicGeoposition);
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
            // Request permission to access bluetooth
            try
            {
                DeviceInformationCollection deviceInfoCollection =
                    await DeviceInformation.FindAllAsync(RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort));
                if (deviceInfoCollection.Count() > 0)
                {
                    DeviceInformation device = deviceInfoCollection[0];
                    await RfcommDeviceService.FromIdAsync(device.Id);
                }
            }
            catch (Exception) { }
            // Request permission to access location
            var accessStatus = await Geolocator.RequestAccessAsync();

            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:

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

            try
            {
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
            catch (Exception ex)
            {
                // Sometimes while creating ExtendedExecution session you get Resource not ready exception. 
                Logger.Instance.Report(ex);
                Acr.UserDialogs.UserDialogs.Instance.Alert("Will not be able to execute app in the background.",
                        "Background execution session failed.", "OK");
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
                        break;

                    case ExtendedExecutionRevokedReason.SystemPolicy:
                        Acr.UserDialogs.UserDialogs.Instance.Alert("Extended execution revoked due to system policy.",
                                        "Background Execution revoked.", "OK");
                        break;
                }

                ClearExtendedExecution();
                await BeginExtendedExecution();
            });
        }


        private async void StartRecordBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel?.CurrentPosition == null || ViewModel.IsBusy || recordButtonIsBusy)
                return;

            recordButtonIsBusy = true;

            var basicGeoposition = new BasicGeoposition()
            {
                Latitude = ViewModel.CurrentPosition.Latitude,
                Longitude = ViewModel.CurrentPosition.Longitude
            };

            if (ViewModel.IsRecording)
            {
                // Need to update Map UI before we start saving. So that the entire trip is visible. 
                UpdateMap_PositionChanged(basicGeoposition);
               
                // Changing the record button icon and andding end marker earlier to notify user about the stop process. 
                recordButtonImage = new BitmapImage(new Uri("ms-appx:///Assets/StartRecord.png", UriKind.Absolute));
                OnPropertyChanged(nameof(RecordButtonImage));
                AddEndMarker(basicGeoposition);

                if (!await ViewModel.StopRecordingTrip())
                {
                    recordButtonIsBusy = false;
                    return;
                }

                await ViewModel.SaveRecordingTripAsync();

                var recordedTripSummary = ViewModel.TripSummary;
                // Launch Trip Summary Page. 
                Frame.Navigate(typeof(TripSummaryView), recordedTripSummary);
            }
            else
            {
                if (!await ViewModel.StartRecordingTrip())
                {
                    recordButtonIsBusy = false;
                    return;
                }

                if (ViewModel.CurrentTrip.HasSimulatedOBDData)
                    App.SetTitle("CURRENT TRIP (SIMULATED OBD)");

                // Update UI to start recording.
                recordButtonImage = new BitmapImage(new Uri("ms-appx:///Assets/StopRecord.png", UriKind.Absolute));
                OnPropertyChanged(nameof(RecordButtonImage));
                // Update the Map with StartMarker, Path
                AddStartMarker(basicGeoposition);
                UpdateMap_PositionChanged(basicGeoposition);
                UpdateStats();
            }
            recordButtonIsBusy = false;
        }

        private async void UpdateMap_PositionChanged(BasicGeoposition basicGeoposition)
        {
     
            if (ViewModel.IsBusy)
                return;
 
            // To update the carIcon first find it if it exists in MapElements already. 
            // If it exists just update the existing one with new location and image 
            await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                MapIcon _carIcon = null;
                // Find if there is a MapIcon with title Car
                if (MyMap.MapElements != null)
                {
                    var mapIcons = MyMap.MapElements.OfType<MapIcon>().ToList();
                    foreach (var item in mapIcons)
                    {
                        if (item.Title == "Car")
                            _carIcon = item;
                    }
                }

                if (_carIcon == null)
                {
                    // Car Icon is currently not present. So add it. 
                    _carIcon = new MapIcon
                {
                        NormalizedAnchorPoint = new Point(0.5, 0.5),
                        ZIndex = 4,
                        CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible,
                        Title = "Car"
                };
                    MyMap.MapElements.Add(_carIcon);
                }

                // Update the icon of the car based on the recording status
                if (ViewModel.IsRecording)
                    _carIcon.Image =
                        RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/RedCar.png"));
                else
                    _carIcon.Image =
                        RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/BlueCar.png"));

                // Update the location
                _carIcon.Location = new Geopoint(basicGeoposition);
                MyMap.Center = _carIcon.Location;

            // Add Path if we are recording 
                DrawPath(basicGeoposition);
            });
        }

        private async void AddStartMarker(BasicGeoposition basicGeoposition)
        {
            if (!ViewModel.IsRecording || ViewModel.CurrentTrip.Points.Count == 0)
                return;

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // First point of the trip will be Start Position. 
                MapIcon mapStartIcon = new MapIcon
                {
                    Location = new Geopoint(basicGeoposition),
                    NormalizedAnchorPoint = new Point(0.5, 0.5),
                    Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/A100.png")),
                    ZIndex = 3,
                    CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible
                };
                MyMap.MapElements.Add(mapStartIcon);
            });
        }

        private async void DrawPath(BasicGeoposition basicGeoposition)
        {
            if (!ViewModel.IsRecording || ViewModel.CurrentTrip?.Points?.Count == 0)
                return;

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (MyMap == null)
                    return;
                
                if (Locations.Count == 0)
                {
                Locations =
                    new List<BasicGeoposition>(
                        ViewModel.CurrentTrip?.Points?.Select(
                            s => new BasicGeoposition() { Latitude = s.Latitude, Longitude = s.Longitude }));

                    // If the viewmodel still has not added this point, then add it locally to the Locations collection.
                    if (Locations.Count == 0)
                        Locations.Add(basicGeoposition);
                }
                else
                    Locations.Add(basicGeoposition);

                // Check if _mapPolyline is already in MapElements
                var _mapPolyline = MyMap.MapElements.OfType<MapPolyline>().FirstOrDefault();

                if (_mapPolyline == null)
                {
                    // Polyline does not exist. Create a new path and add it.
                    _mapPolyline = new MapPolyline
                    {
                        StrokeColor = Colors.Red,
                        StrokeThickness = 3,
                        Visible = true,
                        Path = new Geopath(Locations)
                    };
                    MyMap.MapElements.Add(_mapPolyline);
                }
                else
                {
                    // Set the path of the already added polyline to new locations
                    _mapPolyline.Path = new Geopath(Locations);
                }
            });
        }
 
        private async void UpdateMapView(BasicGeoposition basicGeoposition)
        {
            var geoPoint = new Geopoint(basicGeoposition);
            if (!ViewModel.IsBusy)
                {
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
            MyMap.MapElements.Clear();
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
                    Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/B100.png")),
                    ZIndex = 3,
                    CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible
                };
                MyMap.MapElements.Add(mapEndIcon);
            });
        }
    }
}
