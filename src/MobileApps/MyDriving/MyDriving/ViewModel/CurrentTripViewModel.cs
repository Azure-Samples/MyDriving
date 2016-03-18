// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MyDriving.Helpers;
using MyDriving.DataObjects;
using MyDriving.Utils;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using MvvmHelpers;
using Plugin.Media.Abstractions;
using Plugin.Media;
using Plugin.DeviceInfo;
using MyDriving.Services;

namespace MyDriving.ViewModel
{
    public class CurrentTripViewModel : ViewModelBase
    {
        readonly OBDDataProcessor _obdDataProcessor;
        readonly List<Photo> _photos;
        string _distance = "0.0";

        string _distanceUnits = "miles";

        string _elapsedTime = "0s";

        string _engineLoad = "N/A";

        string _fuelConsumption = "N/A";

        double _fuelConsumptionRate;

        string _fuelConsumptionUnits = "Gallons";

        bool _hasEngineLoad;
        bool _isRecording;

        Position _position;

        ICommand _startTrackingTripCommand;

        ICommand _stopTrackingTripCommand;

        ICommand _takePhotoCommand;

        public CurrentTripViewModel()
        {
            CurrentTrip = new Trip
            {
                UserId = Settings.Current.UserUID,
                Points = new ObservableRangeCollection<TripPoint>()
            };
            _photos = new List<Photo>();
            _fuelConsumptionRate = 0;
            FuelConsumptionUnits = Settings.MetricUnits ? "Liters" : "Gallons";
            DistanceUnits = Settings.MetricDistance ? "Kilometers" : "Miles";
            ElapsedTime = "0s";
            Distance = "0.0";
            FuelConsumption = "N/A";
            EngineLoad = "N/A";
            _obdDataProcessor = OBDDataProcessor.GetProcessor();
        }

        public Trip CurrentTrip { get; private set; }

        public bool IsRecording
        {
            get { return _isRecording; }
            private set { SetProperty(ref _isRecording, value); }
        }

        public Position CurrentPosition
        {
            get { return _position; }
            set { SetProperty(ref _position, value); }
        }

        public string ElapsedTime
        {
            get { return _elapsedTime; }
            set { SetProperty(ref _elapsedTime, value); }
        }

        public string Distance
        {
            get { return _distance; }
            set { SetProperty(ref _distance, value); }
        }

        public string DistanceUnits
        {
            get { return _distanceUnits; }
            set { SetProperty(ref _distanceUnits, value); }
        }

        public string FuelConsumption
        {
            get { return _fuelConsumption; }
            set { SetProperty(ref _fuelConsumption, value); }
        }

        public string FuelConsumptionUnits
        {
            get { return _fuelConsumptionUnits; }
            set { SetProperty(ref _fuelConsumptionUnits, value); }
        }

        public string EngineLoad
        {
            get { return _engineLoad; }
            set { SetProperty(ref _engineLoad, value); }
        }

        public bool NeedSave { get; set; }
        public IGeolocator Geolocator => CrossGeolocator.Current;
        public IMedia Media => CrossMedia.Current;

        public TripSummaryViewModel TripSummary { get; set; }

        public ICommand StartTrackingTripCommand =>
            _startTrackingTripCommand ??
            (_startTrackingTripCommand = new RelayCommand(async () => await ExecuteStartTrackingTripCommandAsync()));

        public ICommand StopTrackingTripCommand =>
            _stopTrackingTripCommand ??
            (_stopTrackingTripCommand = new RelayCommand(async () => await ExecuteStopTrackingTripCommandAsync()));

        public ICommand TakePhotoCommand =>
            _takePhotoCommand ??
            (_takePhotoCommand = new RelayCommand(async () => await ExecuteTakePhotoCommandAsync()));

        public async Task<bool> StartRecordingTrip()
        {
            if (IsBusy || IsRecording)
                return false;

            try
            {
                if (CurrentPosition == null)
                {
                    if (CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.Android ||
                        CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.iOS ||
                        CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.WindowsPhone)
                    {
                        Acr.UserDialogs.UserDialogs.Instance.Toast(
                            new Acr.UserDialogs.ToastConfig(Acr.UserDialogs.ToastEvent.Success,
                                "Waiting for current location.")
                            {
                                Duration = TimeSpan.FromSeconds(3),
                                TextColor = System.Drawing.Color.White,
                                BackgroundColor = System.Drawing.Color.FromArgb(96, 125, 139)
                            });
                    }

                    return false;
                }

                //Connect to the OBD device
                await _obdDataProcessor.ConnectToObdDevice(true);

                CurrentTrip.HasSimulatedOBDData = _obdDataProcessor.IsObdDeviceSimulated;

                CurrentTrip.RecordedTimeStamp = DateTime.UtcNow;

                IsRecording = true;

                //add start point
                CurrentTrip.Points.Add(new TripPoint
                {
                    RecordedTimeStamp = DateTime.UtcNow,
                    Latitude = CurrentPosition.Latitude,
                    Longitude = CurrentPosition.Longitude,
                    Sequence = CurrentTrip.Points.Count,
                });
            }
            catch (Exception ex)
            {
                Logger.Instance.Report(ex);
            }

            return IsRecording;
        }

        public async Task<bool> SaveRecordingTripAsync(string name = "")
        {
            if (IsRecording)
                return false;

            var track = Logger.Instance.TrackTime("SaveRecording");
            IsBusy = true;

            var progress = Acr.UserDialogs.UserDialogs.Instance.Loading("Saving trip...", show: false,
                maskType: Acr.UserDialogs.MaskType.Clear);

            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    CurrentTrip.Name = DateTime.Now.ToString("d") + " " + DateTime.Now.ToString("t");
                    var result = await Acr.UserDialogs.UserDialogs.Instance.PromptAsync(new Acr.UserDialogs.PromptConfig
                    {
                        Text = CurrentTrip.Name,
                        OkText = "OK",
                        IsCancellable = false,
                        Title = "Name of Trip",
                        Message = String.Empty,
                        Placeholder = String.Empty
                    });
                    CurrentTrip.Name = result?.Text ?? string.Empty;
                }
                else
                {
                    CurrentTrip.Name = name;
                }
                track?.Start();
                progress?.Show();

                CurrentTrip.MainPhotoUrl =
                    $"http://dev.virtualearth.net/REST/V1/Imagery/Map/Road/{CurrentPosition.Latitude.ToString(CultureInfo.InvariantCulture)},{CurrentPosition.Longitude.ToString(CultureInfo.InvariantCulture)}/15?mapSize=500,220&key=J0glkbW63LO6FSVcKqr3~_qnRwBJkAvFYgT0SK7Nwyw~An57C8LonIvP00ncUAQrkNd_PNYvyT4-EnXiV0koE1KdDddafIAPFaL7NzXnELRn";
                CurrentTrip.Rating = 90;

                await StoreManager.TripStore.InsertAsync(CurrentTrip);

                foreach (var photo in _photos)
                {
                    photo.TripId = CurrentTrip.Id;
                    await StoreManager.PhotoStore.InsertAsync(photo);
                }

                CurrentTrip = new Trip {Points = new ObservableRangeCollection<TripPoint>()};

                ElapsedTime = "0s";
                Distance = "0.0";
                FuelConsumption = "N/A";
                EngineLoad = "N/A";

                OnPropertyChanged(nameof(CurrentTrip));
                OnPropertyChanged("Stats");
                NeedSave = false;
                return true;
            }
            catch (Exception ex)
            {
                Logger.Instance.Report(ex);
            }
            finally
            {
                track?.Stop();
                IsBusy = false;
                progress?.Dispose();
            }

            return false;
        }

        public async Task<bool> StopRecordingTrip()
        {
            if (IsBusy || !IsRecording)
                return false;


            //always will have 1 point, so we can stop.


            CurrentTrip.EndTimeStamp = DateTime.UtcNow;

            try
            {
                //Disconnect from the OBD device; if still trying to connect, stop polling for the device
                await _obdDataProcessor.DisconnectFromObdDevice();
            }
            catch (Exception ex)
            {
                Logger.Instance.Report(ex);
            }

            var poiList = (List<POI>)await StoreManager.POIStore.GetItemsAsync();
            CurrentTrip.HardStops = poiList.Where(p => p.POIType == POIType.HardBrake).Count();
            CurrentTrip.HardAccelerations = poiList.Where(p => p.POIType == POIType.HardAcceleration).Count();

            TripSummary = new TripSummaryViewModel
            {
                TotalTime = (CurrentTrip.EndTimeStamp - CurrentTrip.RecordedTimeStamp).TotalSeconds,
                TotalDistance = CurrentTrip.Distance,
                FuelUsed = CurrentTrip.FuelUsed,
                MaxSpeed = CurrentTrip.Points.Max(s => s.Speed),
                HardStops = CurrentTrip.HardStops,
                HardAccelerations = CurrentTrip.HardAccelerations,
                Date = DateTime.UtcNow
            };

            IsRecording = false;
            NeedSave = true;
            return true;
        }

        public async Task ExecuteStartTrackingTripCommandAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                if (Geolocator.IsListening)
                {
                    await Geolocator.StopListeningAsync();
                }

                if (Geolocator.IsGeolocationAvailable && (Settings.Current.FirstRun || Geolocator.IsGeolocationEnabled))
                {
                    Geolocator.AllowsBackgroundUpdates = true;
                    Geolocator.DesiredAccuracy = 25;

                    Geolocator.PositionChanged += Geolocator_PositionChanged;
                    //every second, 5 meters
                    await Geolocator.StartListeningAsync(1000, 0);
                }
                else
                {
                    Acr.UserDialogs.UserDialogs.Instance.Alert(
                        "Please ensure that geolocation is enabled and permissions are allowed for MyDriving to start a recording.",
                        "Geolocation Disabled", "OK");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Report(ex);
            }
        }

        public async Task ExecuteStopTrackingTripCommandAsync()
        {
            if (IsBusy || !IsRecording)
                return;

            try
            {
                //Unsubscribe because we were recording and it is alright
                Geolocator.PositionChanged -= Geolocator_PositionChanged;
                await Geolocator.StopListeningAsync();
            }
            catch (Exception ex)
            {
                Logger.Instance.Report(ex);
            }
        }

        async Task AddOBDDataToPoint(TripPoint point)
        {
            //Read data from the OBD device
            point.HasOBDData = false;
            var obdData = await _obdDataProcessor.ReadOBDData();

            if (obdData != null)
            {
                double speed = -255,
                    rpm = -255,
                    efr = -255,
                    el = -255,
                    stfb = -255,
                    ltfb = -255,
                    fr = -255,
                    tp = -255,
                    rt = -255,
                    dis = -255,
                    rtp = -255;
                var vin = String.Empty;

                if (obdData.ContainsKey("el") && !string.IsNullOrWhiteSpace(obdData["el"]))
                {
                    double.TryParse(obdData["el"], out el);
                    _hasEngineLoad = el != -255;
                }
                if (obdData.ContainsKey("stfb"))
                    double.TryParse(obdData["stfb"], out stfb);
                if (obdData.ContainsKey("ltfb"))
                    double.TryParse(obdData["ltfb"], out ltfb);
                if (obdData.ContainsKey("fr"))
                {
                    double.TryParse(obdData["fr"], out fr);
                    if (fr != -255)
                    {
                        _fuelConsumptionRate = fr;
                    }
                    else
                        ;
                }
                if (obdData.ContainsKey("tp"))
                    double.TryParse(obdData["tp"], out tp);
                if (obdData.ContainsKey("rt"))
                    double.TryParse(obdData["rt"], out rt);
                if (obdData.ContainsKey("dis"))
                    double.TryParse(obdData["dis"], out dis);
                if (obdData.ContainsKey("rtp"))
                    double.TryParse(obdData["rtp"], out rtp);
                if (obdData.ContainsKey("spd"))
                    double.TryParse(obdData["spd"], out speed);
                if (obdData.ContainsKey("rpm"))
                    double.TryParse(obdData["rpm"], out rpm);
                if (obdData.ContainsKey("efr") && !string.IsNullOrWhiteSpace(obdData["efr"]))
                {
                    if (!double.TryParse(obdData["efr"], out efr))
                        efr = -255;
                }
                else
                {
                    efr = -255;
                }
                if (obdData.ContainsKey("vin"))
                    vin = obdData["vin"];

                point.EngineLoad = el;
                point.ShortTermFuelBank = stfb;
                point.LongTermFuelBank = ltfb;
                point.MassFlowRate = fr;
                point.ThrottlePosition = tp;
                point.Runtime = rt;
                point.DistanceWithMalfunctionLight = dis;
                point.RelativeThrottlePosition = rtp;
                point.Speed = speed;
                point.RPM = rpm;
                point.EngineFuelRate = efr;
                point.VIN = vin;

                foreach (var kvp in obdData)
                    Logger.Instance.WriteLine($"{kvp.Key} {kvp.Value}");

                point.HasOBDData = true;
            }
        }

        async void Geolocator_PositionChanged(object sender, PositionEventArgs e)
        {
            // Only update the route if we are meant to be recording coordinates
            if (IsRecording)
            {
                var userLocation = e.Position;

                var point = new TripPoint
                {
                    TripId = CurrentTrip.Id,
                    RecordedTimeStamp = DateTime.UtcNow,
                    Latitude = userLocation.Latitude,
                    Longitude = userLocation.Longitude,
                    Sequence = CurrentTrip.Points.Count,
                };

                _hasEngineLoad = false;

                //Add OBD data
                point.HasSimulatedOBDData = _obdDataProcessor.IsObdDeviceSimulated;
                await AddOBDDataToPoint(point);

                CurrentTrip.Points.Add(point);

                try
                {
                    //Push the trip data packaged with the OBD data to the IOT Hub
                    _obdDataProcessor.SendTripPointToIOTHub(CurrentTrip.Id, CurrentTrip.UserId, point);
                }
                catch (Exception ex)
                {
                    Logger.Instance.Report(ex);
                }

                if (CurrentTrip.Points.Count > 1)
                {
                    var previous = CurrentTrip.Points[CurrentTrip.Points.Count - 2];
                    CurrentTrip.Distance += DistanceUtils.CalculateDistance(userLocation.Latitude,
                        userLocation.Longitude, previous.Latitude, previous.Longitude);
                    Distance = CurrentTrip.TotalDistanceNoUnits;

                    //calculate gas usage
                    var timeDif1 = point.RecordedTimeStamp - previous.RecordedTimeStamp;
                    CurrentTrip.FuelUsed += _fuelConsumptionRate*0.00002236413*timeDif1.Seconds;
                    FuelConsumption = Settings.MetricUnits
                        ? (CurrentTrip.FuelUsed*3.7854).ToString("N2")
                        : CurrentTrip.FuelUsed.ToString("N2");
                }
                else
                {
                    CurrentTrip.FuelUsed = 0;
                    FuelConsumption = "N/A";
                }

                var timeDif = point.RecordedTimeStamp - CurrentTrip.RecordedTimeStamp;

                //track seconds, minutes, then hours
                if (timeDif.TotalMinutes < 1)
                    ElapsedTime = $"{timeDif.Seconds}s";
                else if (timeDif.TotalHours < 1)
                    ElapsedTime = $"{timeDif.Minutes}m {timeDif.Seconds}s";
                else
                    ElapsedTime = $"{(int) timeDif.TotalHours}h {timeDif.Minutes}m {timeDif.Seconds}s";

                if (_hasEngineLoad)
                    EngineLoad = $"{(int) point.EngineLoad}%";

                FuelConsumptionUnits = Settings.MetricUnits ? "Liters" : "Gallons";
                DistanceUnits = Settings.MetricDistance ? "Kilometers" : "Miles";

                OnPropertyChanged("Stats");
            }

            CurrentPosition = e.Position;
        }

        async Task ExecuteTakePhotoCommandAsync()
        {
            try
            {
                await Media.Initialize();

                if (!Media.IsCameraAvailable || !Media.IsTakePhotoSupported)
                {
                    Acr.UserDialogs.UserDialogs.Instance.Alert(
                        "Please ensure that camera is enabled and permissions are allowed for MyDriving to take photos.",
                        "Camera Disabled", "OK");

                    return;
                }

                var locationTask = Geolocator.GetPositionAsync(2500);
                var photo = await Media.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    DefaultCamera = CameraDevice.Rear,
                    Directory = "MyDriving",
                    Name = "MyDriving_",
                    SaveToAlbum = true,
                    PhotoSize = PhotoSize.Small
                });

                if (photo == null)
                {
                    return;
                }

                if (CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.Android ||
                    CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.iOS)
                {
                    Acr.UserDialogs.UserDialogs.Instance.Toast(
                        new Acr.UserDialogs.ToastConfig(Acr.UserDialogs.ToastEvent.Success, "Photo taken!")
                        {
                            Duration = TimeSpan.FromSeconds(3),
                            TextColor = System.Drawing.Color.White,
                            BackgroundColor = System.Drawing.Color.FromArgb(96, 125, 139)
                        });
                }

                var local = await locationTask;
                var photoDb = new Photo
                {
                    PhotoUrl = photo.Path,
                    Latitude = local.Latitude,
                    Longitude = local.Longitude,
                    TimeStamp = DateTime.UtcNow
                };

                _photos.Add(photoDb);
                photo.Dispose();
            }
            catch (Exception ex)
            {
                Logger.Instance.Report(ex);
            }
        }
    }
}