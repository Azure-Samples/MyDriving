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
using MyDriving.Utils.Helpers;
using MyDriving.AzureClient;
using MyDriving.Utils.Interfaces;

namespace MyDriving.ViewModel
{
    public class CurrentTripViewModel : ViewModelBase
    {
        readonly OBDDataProcessor obdDataProcessor;
        readonly List<Photo> photos;
        string distance = "0.0";

        string distanceUnits = "miles";

        string elapsedTime = "0s";

        string engineLoad = "N/A";

        string fuelConsumption = "N/A";

        double fuelConsumptionRate;

        string fuelConsumptionUnits = "Gallons";

        bool isRecording;

        Position position;

        ICommand startTrackingTripCommand;

        ICommand stopTrackingTripCommand;

        ICommand takePhotoCommand;

        public CurrentTripViewModel()
        {
            CurrentTrip = new Trip
            {
                UserId = Settings.Current.UserUID,
                Points = new ObservableRangeCollection<TripPoint>()
            };
            photos = new List<Photo>();
            fuelConsumptionRate = 0;
            FuelConsumptionUnits = Settings.MetricUnits ? "Liters" : "Gallons";
            DistanceUnits = Settings.MetricDistance ? "Kilometers" : "Miles";
            ElapsedTime = "0s";
            Distance = "0.0";
            FuelConsumption = "N/A";
            EngineLoad = "N/A";
            obdDataProcessor = OBDDataProcessor.GetProcessor();
        }

        public Trip CurrentTrip { get; private set; }

        public bool IsRecording
        {
            get { return isRecording; }
            private set { SetProperty(ref isRecording, value); }
        }

        public Position CurrentPosition
        {
            get { return position; }
            set { SetProperty(ref position, value); }
        }

        public string ElapsedTime
        {
            get { return elapsedTime; }
            set { SetProperty(ref elapsedTime, value); }
        }

        public string Distance
        {
            get { return distance; }
            set { SetProperty(ref distance, value); }
        }

        public string DistanceUnits
        {
            get { return distanceUnits; }
            set { SetProperty(ref distanceUnits, value); }
        }

        public string FuelConsumption
        {
            get { return fuelConsumption; }
            set { SetProperty(ref fuelConsumption, value); }
        }

        public string FuelConsumptionUnits
        {
            get { return fuelConsumptionUnits; }
            set { SetProperty(ref fuelConsumptionUnits, value); }
        }

        public string EngineLoad
        {
            get { return engineLoad; }
            set { SetProperty(ref engineLoad, value); }
        }

        public bool NeedSave { get; set; }
        public IGeolocator Geolocator => CrossGeolocator.Current;
        public IMedia Media => CrossMedia.Current;

        public TripSummaryViewModel TripSummary { get; set; }

        public ICommand StartTrackingTripCommand =>
            startTrackingTripCommand ??
            (startTrackingTripCommand = new RelayCommand(async () => await ExecuteStartTrackingTripCommandAsync()));

        public ICommand StopTrackingTripCommand =>
            stopTrackingTripCommand ??
            (stopTrackingTripCommand = new RelayCommand(async () => await ExecuteStopTrackingTripCommandAsync()));

        public ICommand TakePhotoCommand =>
            takePhotoCommand ??
            (takePhotoCommand = new RelayCommand(async () => await ExecuteTakePhotoCommandAsync()));

        public async Task<bool> StartRecordingTrip()
        {
            if (IsBusy || IsRecording)
                return false;

            //Since the current trip screen is typically the first screen opened, let's do an up-front check to ensure the user is authenticated
            await AzureClient.AzureClient.CheckIsAuthTokenValid();

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
                if (obdDataProcessor != null)
                {
                    await obdDataProcessor.ConnectToObdDevice(true);

                    CurrentTrip.HasSimulatedOBDData = obdDataProcessor.IsObdDeviceSimulated;
                }

                CurrentTrip.RecordedTimeStamp = DateTime.UtcNow;

                IsRecording = true;
                Logger.Instance.Track("StartRecording");
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

            if (CurrentTrip.Points?.Count < 1)
            {
                Logger.Instance.Track("Attempt to save a trip with no points!");
                return false;
            }
            IsBusy = true;
            var tripId = CurrentTrip.Id;

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

                ProgressDialogManager.LoadProgressDialog("Saving trip...");

                if (Logger.BingMapsAPIKey != "____BingMapsAPIKey____")
                {

                    CurrentTrip.MainPhotoUrl =
                        $"http://dev.virtualearth.net/REST/V1/Imagery/Map/Road/{CurrentPosition.Latitude.ToString(CultureInfo.InvariantCulture)},{CurrentPosition.Longitude.ToString(CultureInfo.InvariantCulture)}/15?mapSize=500,220&key={Logger.BingMapsAPIKey}";
                }
                else
                {
                    CurrentTrip.MainPhotoUrl = string.Empty;
                }
                CurrentTrip.Rating = 90;

                await StoreManager.TripStore.InsertAsync(CurrentTrip);

                foreach (var photo in photos)
                {
                    photo.TripId = CurrentTrip.Id;
                    await StoreManager.PhotoStore.InsertAsync(photo);
                }

                CurrentTrip = new Trip { Points = new ObservableRangeCollection<TripPoint>() };

                ElapsedTime = "0s";
                Distance = "0.0";
                FuelConsumption = "N/A";
                EngineLoad = "N/A";

                OnPropertyChanged(nameof(CurrentTrip));
                OnPropertyChanged("Stats");
                NeedSave = false;
                Logger.Instance.Track("SaveRecording");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Instance.Report(ex);
            }
            finally
            {
                
                IsBusy = false;

                ProgressDialogManager.DisposeProgressDialog();
            }
            Logger.Instance.Track("SaveRecording failed for trip " + tripId);
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
                if (obdDataProcessor != null)
                {
                    //Disconnect from the OBD device; if still trying to connect, stop polling for the device
                    await obdDataProcessor.DisconnectFromObdDevice();
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Report(ex);
            }

            List<POI> poiList = new List<POI>();
            try
            {
                poiList = new List<POI>(await StoreManager.POIStore.GetItemsAsync(CurrentTrip.Id));
            }
            catch (Exception ex)
            {
                //Intermittently, Sqlite will cause a crash for WinPhone saying "unable to set temporary directory"
                //If this call fails, simply use an empty list of POIs
                Logger.Instance.Track("Unable to get POI Store Items.");
                Logger.Instance.Report(ex);
            }
           
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
            Logger.Instance.Track("StopRecording");
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

				if (Geolocator.IsGeolocationAvailable && (CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.iOS || Geolocator.IsGeolocationEnabled))
                {
                    Geolocator.AllowsBackgroundUpdates = true;
                    Geolocator.DesiredAccuracy = 25;

                    Geolocator.PositionChanged += Geolocator_PositionChanged;
                    //every 3 second, 5 meters
                    await Geolocator.StartListeningAsync(3000, 5);
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
            Dictionary<string, string> obdData = null;

            if (obdDataProcessor != null)
                obdData = await obdDataProcessor.ReadOBDData();

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
                    if (!double.TryParse(obdData["el"], out el))
                        el = -255;
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
                        fuelConsumptionRate = fr;
                    }
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

                #if DEBUG
                foreach (var kvp in obdData)
                    Logger.Instance.Track($"{kvp.Key} {kvp.Value}");
                #endif

                point.HasOBDData = true;
            }
        }


        async void Geolocator_PositionChanged(object sender, PositionEventArgs e)
        {
            // Only update the route if we are meant to be recording coordinates
            if (IsRecording)
            {
                var userLocation = e.Position;

                TripPoint previous = null;
                double newDistance = 0;
                if (CurrentTrip.Points.Count > 1)
                {
                    previous = CurrentTrip.Points[CurrentTrip.Points.Count - 1];
                    newDistance = DistanceUtils.CalculateDistance(userLocation.Latitude,
                        userLocation.Longitude, previous.Latitude, previous.Longitude);

                    if (newDistance > 4) // if more than 4 miles then gps is off don't use
                        return;
                }

                var point = new TripPoint
                    {
                        TripId = CurrentTrip.Id,
                        RecordedTimeStamp = DateTime.UtcNow,
                        Latitude = userLocation.Latitude,
                        Longitude = userLocation.Longitude,
                        Sequence = CurrentTrip.Points.Count,
                        Speed = -255,
                        RPM = -255,
                        ShortTermFuelBank = -255,
                        LongTermFuelBank = -255,
                        ThrottlePosition = -255,
                        RelativeThrottlePosition = -255,
                        Runtime = -255,
                        DistanceWithMalfunctionLight = -255,
                        EngineLoad = -255,
                        MassFlowRate = -255,
                        EngineFuelRate = -255,
                        VIN = "-255"
                    };
       
                //Add OBD data
                if (obdDataProcessor != null)
                    point.HasSimulatedOBDData = obdDataProcessor.IsObdDeviceSimulated;
                await AddOBDDataToPoint(point);

                CurrentTrip.Points.Add(point);

                try
                {
                    if (obdDataProcessor != null)
                    {
                        //Push the trip data packaged with the OBD data to the IOT Hub
                        obdDataProcessor.SendTripPointToIOTHub(CurrentTrip.Id, CurrentTrip.UserId, point);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.Report(ex);
                }

                if (CurrentTrip.Points.Count > 1 && previous != null)
                {
                    CurrentTrip.Distance += newDistance;
                    Distance = CurrentTrip.TotalDistanceNoUnits;

                    //calculate gas usage
                    var timeDif1 = point.RecordedTimeStamp - previous.RecordedTimeStamp;
                    CurrentTrip.FuelUsed += fuelConsumptionRate * 0.00002236413 * timeDif1.TotalSeconds;
                    if (CurrentTrip.FuelUsed == 0)
                        FuelConsumption = "N/A";
                    else
                        FuelConsumption = Settings.MetricUnits
                            ? (CurrentTrip.FuelUsed * 3.7854).ToString("N2")
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
                    ElapsedTime = $"{(int)timeDif.TotalHours}h {timeDif.Minutes}m {timeDif.Seconds}s";

                if (point.EngineLoad != -255)
                    EngineLoad = $"{(int)point.EngineLoad}%";

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

                photos.Add(photoDb);
                photo.Dispose();
            }
            catch (Exception ex)
            {
                Logger.Instance.Report(ex);
            }
        }
    }
}