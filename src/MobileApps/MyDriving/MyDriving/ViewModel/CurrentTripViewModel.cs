using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MyDriving.Helpers;

using MyDriving.DataObjects;
using MyDriving.Utils;
using MyDriving.Model;

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
        public Trip CurrentTrip { get; private set; }
        List<Photo> photos;
        OBDDataProcessor obdDataProcessor;

        double totalConsumption = 0;
        double totalConsumptionPoints = 0;

        bool isRecording;
		public bool IsRecording
        {
            get { return isRecording; }
            private set { SetProperty(ref isRecording, value); }
        }

        Position position;
        public Position CurrentPosition
        {
            get { return position; }
            set { SetProperty(ref position, value); }
        }

        string elapsedTime = "0s";
        public string ElapsedTime
        {
            get { return elapsedTime; }
            set { SetProperty(ref elapsedTime, value); }
        }

        string distance = "0.0";
        public string Distance
        {
            get { return distance; }
            set { SetProperty(ref distance, value); }
        }

        string distanceUnits = "miles";
        public string DistanceUnits
        {
            get { return distanceUnits; }
            set { SetProperty(ref distanceUnits, value); }
        }

        string fuelConsumption = "N/A";
        public string FuelConsumption
        {
            get { return fuelConsumption; }
            set { SetProperty(ref fuelConsumption, value); }
        }

        string fuelConsumptionUnits = "Gallons";
        public string FuelConsumptionUnits
        {
            get { return fuelConsumptionUnits; }
            set { SetProperty(ref fuelConsumptionUnits, value); }
        }

        string engineLoad = "N/A";
        public string EngineLoad
        {
            get { return engineLoad; }
            set { SetProperty(ref engineLoad, value); }
        }

		public CurrentTripViewModel()
		{
            CurrentTrip = new Trip();
            CurrentTrip.UserId = Settings.Current.UserUID;
            CurrentTrip.Points = new ObservableRangeCollection<TripPoint>();
            photos = new List<Photo>();

            FuelConsumptionUnits = Settings.MetricUnits ? "Liters" : "Gallons";
            DistanceUnits = Settings.MetricDistance ? "Kilometers" : "Miles";
            ElapsedTime = "0s";
            Distance = "0.0";
            FuelConsumption = "N/A";
            EngineLoad = "N/A";
            obdDataProcessor = OBDDataProcessor.GetProcessor();
		}

        public bool NeedSave { get; set; }
        public IGeolocator Geolocator => CrossGeolocator.Current;
        public IMedia Media => CrossMedia.Current;

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
                        Acr.UserDialogs.UserDialogs.Instance.Toast(new Acr.UserDialogs.ToastConfig(Acr.UserDialogs.ToastEvent.Success, "Waiting for current location.")
                        {
                            Duration = TimeSpan.FromSeconds(3),
                            TextColor = System.Drawing.Color.White,
                            BackgroundColor = System.Drawing.Color.FromArgb(96, 125, 139)
                        });
                    }
                    
                    return false;
                }

                //Connect to the OBD device
                await obdDataProcessor.ConnectToOBDDevice(true);

				CurrentTrip.HasSimulatedOBDData = obdDataProcessor.IsOBDDeviceSimulated;

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
            finally
            {
            }

            return IsRecording;
        }

        public async Task<bool> SaveRecordingTripAsync(string name = "")
        {
            if (IsRecording)
                return false;
            
            var track = Logger.Instance.TrackTime("SaveRecording");
            IsBusy = true;

            var progress = Acr.UserDialogs.UserDialogs.Instance.Loading("Saving trip...", show: false, maskType: Acr.UserDialogs.MaskType.Clear);

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

                CurrentTrip.MainPhotoUrl = $"http://dev.virtualearth.net/REST/V1/Imagery/Map/Road/{CurrentPosition.Latitude.ToString(CultureInfo.InvariantCulture)},{CurrentPosition.Longitude.ToString(CultureInfo.InvariantCulture)}/15?mapSize=500,220&key=J0glkbW63LO6FSVcKqr3~_qnRwBJkAvFYgT0SK7Nwyw~An57C8LonIvP00ncUAQrkNd_PNYvyT4-EnXiV0koE1KdDddafIAPFaL7NzXnELRn";
                CurrentTrip.Rating = 90;

                await StoreManager.TripStore.InsertAsync(CurrentTrip);

                foreach (var photo in photos)
                {
                    photo.TripId = CurrentTrip.Id;
                    await StoreManager.PhotoStore.InsertAsync(photo);
                }

                CurrentTrip = new Trip();
                CurrentTrip.Points = new ObservableRangeCollection<TripPoint>();

                totalConsumption = 0;
                totalConsumptionPoints = 0;

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

        public TripSummaryViewModel TripSummary { get; set; }
   
        public async Task<bool> StopRecordingTrip()
        {
            if (IsBusy || !IsRecording)
                return false;

        
            //always will have 1 point, so we can stop.


            CurrentTrip.EndTimeStamp = DateTime.UtcNow;

            try
            {
                //Disconnect from the OBD device; if still trying to connect, stop polling for the device
                await obdDataProcessor.DisconnectFromOBDDevice();
            }
            catch(Exception ex)
            {
                Logger.Instance.Report(ex);
            }

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

        ICommand startTrackingTripCommand;
		public ICommand StartTrackingTripCommand =>
		    startTrackingTripCommand ?? (startTrackingTripCommand = new RelayCommand(async () => await ExecuteStartTrackingTripCommandAsync())); 

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
                        Acr.UserDialogs.UserDialogs.Instance.Alert("Please ensure that geolocation is enabled and permissions are allowed for MyDriving to start a recording.",
                                                                   "Geolocation Disabled", "OK");
				}
            }
            catch (Exception ex)
            {
                Logger.Instance.Report(ex);
            }
            finally
            {
            }
        }

        ICommand stopTrackingTripCommand;
		public ICommand StopTrackingTripCommand =>
		stopTrackingTripCommand ?? (stopTrackingTripCommand = new RelayCommand(async () => await ExecuteStopTrackingTripCommandAsync())); 

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
			finally 
			{
			}
		}

        bool hasEngineLoad;
        async Task AddOBDDataToPoint(TripPoint point)
        {
            //Read data from the OBD device
            point.HasOBDData = false;
            var obdData = await obdDataProcessor.ReadOBDData();

            if (obdData != null)
            {
                double speed = -255, rpm = -255, efr = -255, el = -255, stfb = -255, 
                    ltfb = -255, fr = -255, tp = -255, rt = -255, dis = -255, rtp = -255;
                var vin = String.Empty;

				var hasEfr = false;

                if (obdData.ContainsKey("el") && !string.IsNullOrWhiteSpace(obdData["el"]))
                {
                    double.TryParse(obdData["el"], out el);
                    if (el != -255)
                        hasEngineLoad = true;
                    else
                        hasEngineLoad = false;
                }
                if (obdData.ContainsKey("stfb"))
                    double.TryParse(obdData["stfb"], out stfb);
                if (obdData.ContainsKey("ltfb"))
                    double.TryParse(obdData["ltfb"], out ltfb);
                if (obdData.ContainsKey("fr"))
                {
                    double.TryParse(obdData["fr"], out fr);
                    if (fr != -255)
                        hasEfr = true;
                    else
                        hasEfr = false;
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

				if (hasEfr)
				{
					totalConsumption += point.MassFlowRate * 0.3047247;
					totalConsumptionPoints++;
				}
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

                hasEngineLoad = false;

                //Add OBD data
                point.HasSimulatedOBDData = obdDataProcessor.IsOBDDeviceSimulated;
                await AddOBDDataToPoint(point);

                CurrentTrip.Points.Add(point);

                try
                {
                    //Push the trip data packaged with the OBD data to the IOT Hub
                    obdDataProcessor.SendTripPointToIOTHub(CurrentTrip.Id, CurrentTrip.UserId, point);
                }
                catch (Exception ex)
                {
                    Logger.Instance.Report(ex);
                }

                if (CurrentTrip.Points.Count > 1)
                {
                    var previous = CurrentTrip.Points[CurrentTrip.Points.Count - 2];
                    CurrentTrip.Distance += DistanceUtils.CalculateDistance(userLocation.Latitude, userLocation.Longitude, previous.Latitude, previous.Longitude);
                    Distance = CurrentTrip.TotalDistanceNoUnits;
                }

                var timeDif = point.RecordedTimeStamp - CurrentTrip.RecordedTimeStamp;

                //track seconds, minutes, then hours
                if (timeDif.TotalMinutes < 1)
                    ElapsedTime = $"{timeDif.Seconds}s";
                else if (timeDif.TotalHours < 1)
                    ElapsedTime = $"{timeDif.Minutes}m {timeDif.Seconds}s";
                else
                    ElapsedTime = $"{(int)timeDif.TotalHours}h {timeDif.Minutes}m {timeDif.Seconds}s";

                if (totalConsumptionPoints > 0)
                {
                    var fuelUsedLiters = (totalConsumption / totalConsumptionPoints) * timeDif.TotalHours;
                    CurrentTrip.FuelUsed = fuelUsedLiters * .264172;
                    FuelConsumption = Settings.MetricUnits ? fuelUsedLiters.ToString("N2") : CurrentTrip.FuelUsed.ToString("N2");
                }
                else
                {
                    FuelConsumption = "N/A";
                }

                if(hasEngineLoad)
                 EngineLoad = $"{(int)point.EngineLoad}%";
                
                FuelConsumptionUnits = Settings.MetricUnits ? "Liters" : "Gallons";
                DistanceUnits = Settings.MetricDistance ? "Kilometers" : "Miles";

                OnPropertyChanged("Stats");
			}

            CurrentPosition = e.Position;
		}

        ICommand takePhotoCommand;
        public ICommand TakePhotoCommand =>
            takePhotoCommand ?? (takePhotoCommand = new RelayCommand(async () => await ExecuteTakePhotoCommandAsync())); 

        async Task ExecuteTakePhotoCommandAsync()
        {
            try 
            {
                await Media.Initialize();

                if (!Media.IsCameraAvailable || !Media.IsTakePhotoSupported)
                {
                        Acr.UserDialogs.UserDialogs.Instance.Alert("Please ensure that camera is enabled and permissions are allowed for MyDriving to take photos.",
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
                    Acr.UserDialogs.UserDialogs.Instance.Toast(new Acr.UserDialogs.ToastConfig(Acr.UserDialogs.ToastEvent.Success, "Photo taken!")
                    {
                        Duration = TimeSpan.FromSeconds(3),
                        TextColor = System.Drawing.Color.White,
                        BackgroundColor = System.Drawing.Color.FromArgb(96, 125, 139)
                    });
                }

                var local = await locationTask;
                var photoDB = new Photo
                    {
                        PhotoUrl = photo.Path,
                        Latitude = local.Latitude,
                        Longitude = local.Longitude, 
                        TimeStamp = DateTime.UtcNow
                    };

                photos.Add(photoDB);
                photo.Dispose();
            } 
            catch (Exception ex) 
            {
                Logger.Instance.Report(ex);
            }
        }
	}
}
