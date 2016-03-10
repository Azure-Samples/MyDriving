using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MyTrips.Helpers;

using MyTrips.DataObjects;
using MyTrips.Utils;
using MyTrips.Model;

using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using MvvmHelpers;
using Plugin.Media.Abstractions;
using Plugin.Media;
using Plugin.DeviceInfo;
using MyTrips.Services;

namespace MyTrips.ViewModel
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

        string temperature = "N/A";
        public string Temperature
        {
            get { return temperature; }
            set { SetProperty(ref temperature, value); }
        }

		public CurrentTripViewModel()
		{
            CurrentTrip = new Trip();

            CurrentTrip.Points = new ObservableRangeCollection<TripPoint>();
            photos = new List<Photo>();


            FuelConsumptionUnits = Settings.MetricUnits ? "Liters" : "Gallons";
            DistanceUnits = Settings.MetricDistance ? "Kilometers" : "Miles";
            ElapsedTime = "0s";
            Distance = "0.0";
            FuelConsumption = "N/A";
            Temperature = "N/A";
            this.obdDataProcessor = new OBDDataProcessor();
            this.obdDataProcessor.OnOBDDeviceDisconnected += ObdDataProcessor_OnOBDDeviceDisconnected;
		}

        private async void ObdDataProcessor_OnOBDDeviceDisconnected(bool retryToConnect)
        {
            if (retryToConnect)
            {
                await this.obdDataProcessor.ConnectToOBDDevice();
            }
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
                if (this.CurrentPosition == null)
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

                //Connect to the OBD device
                await this.obdDataProcessor.Initialize(this.StoreManager);
                await this.obdDataProcessor.ConnectToOBDDevice();

                //Simulate recording several data points
                /*for (int i = 0; i < 10; i++)
                {

                    var obdData = new Dictionary<string, string>();

                    //Only call for WinPhone\Android for now since the OBD wrapper isn't available yet for ios
                    if (CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.WindowsPhone ||
                        CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.Android)
                    {
                    //Read data from the OBD device and push it to the IOT Hub
                        obdData = this.obdDataProcessor.ReadOBDData();
                    }

                    CurrentTrip.RecordedTimeStamp = DateTime.UtcNow;

                    var point = new TripPoint
                    {
                        RecordedTimeStamp = DateTime.UtcNow,
                        Latitude = CurrentPosition.Latitude,
                        Longitude = CurrentPosition.Longitude,
                        Sequence = CurrentTrip.Points.Count,
                        OBDData = obdData
                    };

                    CurrentTrip.Points.Add(point);
                }*/
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
                    var result = await Acr.UserDialogs.UserDialogs.Instance.PromptAsync("Name of Trip");
                    CurrentTrip.Name = result?.Text ?? string.Empty;
                }
                else
                {
                    CurrentTrip.Name = name;
                }
                track.Start();
                progress?.Show();


                //TODO: use real city here
                CurrentTrip.MainPhotoUrl = $"http://dev.virtualearth.net/REST/V1/Imagery/Map/Road/{CurrentPosition.Latitude.ToString(CultureInfo.InvariantCulture)},{CurrentPosition.Longitude.ToString(CultureInfo.InvariantCulture)}/15?mapSize=500,220&key=J0glkbW63LO6FSVcKqr3~_qnRwBJkAvFYgT0SK7Nwyw~An57C8LonIvP00ncUAQrkNd_PNYvyT4-EnXiV0koE1KdDddafIAPFaL7NzXnELRn";

                CurrentTrip.Rating = 90;

                CurrentTrip.EndTimeStamp = DateTime.UtcNow;

                if (string.IsNullOrWhiteSpace(CurrentTrip.Name))
                    CurrentTrip.Name = DateTime.Now.ToString("d") + DateTime.Now.ToString("t");

                await StoreManager.TripStore.InsertAsync(CurrentTrip);

                foreach (var photo in photos)
                {
                    photo.TripId = CurrentTrip.Id;
                    await StoreManager.PhotoStore.InsertAsync(photo);
                }

                //Store the packaged trip and OBD data locally before attempting to send to the IOT Hub
                await this.obdDataProcessor.AddTripDataPointToBuffer(CurrentTrip);

                //Push the trip data packaged with the OBD data to the IOT Hub
                await this.obdDataProcessor.PushTripDataToIOTHub();

                CurrentTrip = new Trip();
                CurrentTrip.Points = new ObservableRangeCollection<TripPoint>();

                totalConsumption = 0;
                totalConsumptionPoints = 0;

                ElapsedTime = "0s";
                Distance = "0.0";
                FuelConsumption = "N/A";
                Temperature = "N/A";

                photos = new List<Photo>();
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
                track.Stop();
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

            if (CurrentTrip.Points.Count == 0)
            {

                if (CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.Android ||
                        CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.iOS ||
                        CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.WindowsPhone)
                {
                    Acr.UserDialogs.UserDialogs.Instance.Toast(new Acr.UserDialogs.ToastConfig(Acr.UserDialogs.ToastEvent.Success, "Keep driving! Need a few more points.")
                    {
                        Duration = TimeSpan.FromSeconds(3),
                        TextColor = System.Drawing.Color.White,
                        BackgroundColor = System.Drawing.Color.FromArgb(96, 125, 139)
                    });
                }

                return false;
            }

            //Stop reading data from the OBD device
            await this.obdDataProcessor.DisconnectFromOBDDevice();

            TripSummary = new TripSummaryViewModel
            {
                TotalTime = (CurrentTrip.EndTimeStamp - CurrentTrip.RecordedTimeStamp).TotalSeconds,
                TotalDistance = CurrentTrip.Distance,
                FuelUsed = CurrentTrip.FuelUsed,
                MaxSpeed = CurrentTrip.Points.Max(s => s.Speed),
                HardStops = CurrentTrip.HardStops,
                HardAccelerations = CurrentTrip.HardAccelerations
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
                    await Geolocator.StartListeningAsync(1000, 5);
				}
				else
				{
                        Acr.UserDialogs.UserDialogs.Instance.Alert("Please ensure that geolocation is enabled and permissions are allowed for MyTrips to start a recording.",
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

        void Geolocator_PositionChanged(object sender, PositionEventArgs e)
        {
			// Only update the route if we are meant to be recording coordinates
			if (IsRecording)
			{
				var userLocation = e.Position;

                    //Read data from the OBD device and push it to the IOT Hub
                var obdData = this.obdDataProcessor.ReadOBDData();

                var point = new TripPoint
                {
                    RecordedTimeStamp = DateTime.UtcNow,
                    Latitude = userLocation.Latitude,
                    Longitude = userLocation.Longitude,
                    Sequence = CurrentTrip.Points.Count,
                    OBDData = obdData
				};

                if (obdData != null)
                {
                    double speed = 0, barometric = 0, rpm = 0, outside = -1, inside = 0, efr = 0;
                    if(obdData.ContainsKey("spd"))
                        double.TryParse(obdData["spd"], out speed);
                    if(obdData.ContainsKey("bp"))
                        double.TryParse(obdData["bp"], out barometric);
                    if(obdData.ContainsKey("rpm"))
                        double.TryParse(obdData["rpm"], out rpm);
                    if (obdData.ContainsKey("ot") && !string.IsNullOrWhiteSpace(obdData["ot"]))
                        double.TryParse(obdData["ot"], out outside);
                    if(obdData.ContainsKey("it"))
                        double.TryParse(obdData["it"], out inside);
                    if(obdData.ContainsKey("efr"))
                        double.TryParse(obdData["efr"], out efr);

                    point.Speed = speed;
                    point.BarometricPressure = barometric;
                    point.RPM = rpm;
                    point.OutsideTemperature = outside;
                    point.InsideTemperature = inside;
                    point.EngineFuelRate = efr;

                    totalConsumption += point.EngineFuelRate;
                    totalConsumptionPoints++;
                    point.HasOBDData = true;
                }
                else
                {
                    point.HasOBDData = false;
                }

                CurrentTrip.Points.Add(point);

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
                    ElapsedTime = $"{timeDif.Minutes}m";
                else
                    ElapsedTime = $"{(int)timeDif.TotalHours}h {timeDif.Minutes}m";

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

                if(point.OutsideTemperature >= 0)
                    Temperature = point.DisplayTemp;
                
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
                        Acr.UserDialogs.UserDialogs.Instance.Alert("Please ensure that camera is enabled and permissions are allowed for MyTrips to take photos.",
                                                                   "Camera Disabled", "OK");
                    
                    return;
                }

                var locationTask = Geolocator.GetPositionAsync(2500);
                var photo = await Media.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        DefaultCamera = CameraDevice.Rear,
                        Directory = "MyTrips",
                        Name = "MyTrips_",
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
