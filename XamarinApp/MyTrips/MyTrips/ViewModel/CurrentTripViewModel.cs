using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace MyTrips.ViewModel
{
    public class CurrentTripViewModel : ViewModelBase
    {
        public Trip CurrentTrip { get; private set; }
        List<Photo> photos;


        bool isRecording;
		public bool IsRecording
        {
            get { return isRecording; }
            private set { SetProperty(ref isRecording, value); }
        }

        Position position;
        public Position CurrentPosition
        {
            get { return position;}
            set { SetProperty(ref position, value); }
        }

        string elapsedTime = "0:00";
        public string ElapsedTime
        {
            get { return elapsedTime; }
            set { SetProperty(ref elapsedTime, value); }
        }

		public CurrentTripViewModel()
		{
            CurrentTrip = new Trip();

            CurrentTrip.Trail = new ObservableRangeCollection<Trail>();
            photos = new List<Photo>();
		}

		public IGeolocator Geolocator => CrossGeolocator.Current;

        public IMedia Media => CrossMedia.Current;

 
        public Task<bool> StartRecordingTripAsync()
        {
            if (IsBusy || IsRecording)
                return Task.FromResult(false);

            try
            {
                if (CurrentPosition == null)
                {

                    if (CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.Android ||
                        CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.iOS)
                    {
                        Acr.UserDialogs.UserDialogs.Instance.Toast(new Acr.UserDialogs.ToastConfig(Acr.UserDialogs.ToastEvent.Success, "Waiting for current location.")
                        {
                            Duration = TimeSpan.FromSeconds(3),
                            TextColor = System.Drawing.Color.White,
                            BackgroundColor = System.Drawing.Color.FromArgb(96, 125, 139)
                        });
                    }
                    return Task.FromResult(true);
                }
                IsRecording = true;

                CurrentTrip.TimeStamp = DateTime.UtcNow;


                var trail = new Trail
                {
                    TimeStamp = DateTime.UtcNow,
                    Latitude = CurrentPosition.Latitude,
                    Longitude = CurrentPosition.Longitude,
                };


                CurrentTrip.Trail.Add (trail);
            }
            catch(Exception ex)
            {
                Logger.Instance.Report(ex);
            }
            finally
            {
            }

            return Task.FromResult(true);
        }

   
        public async Task<bool> StopRecordingTripAsync()
        {
            if (IsBusy || !IsRecording)
                return false;

          

            var track = Logger.Instance.TrackTime("SaveRecording");
            Acr.UserDialogs.IProgressDialog progress = null;


            if (CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.Android ||
                CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.iOS)
            {
                progress = Acr.UserDialogs.UserDialogs.Instance.Progress("Saving trip...", show: false, maskType: Acr.UserDialogs.MaskType.Clear);
                progress.IsDeterministic = false;
            }
            try
            {
                IsRecording = false;

                var result = await Acr.UserDialogs.UserDialogs.Instance.PromptAsync("Name of Trip");
                CurrentTrip.TripId = result?.Text ?? string.Empty;
                track.Start();
                IsBusy = true;
                progress?.Show();
                #if DEBUG
                await Task.Delay(3000);
#endif

                //TODO: use real city here
#if DEBUG
                CurrentTrip.MainPhotoUrl = "http://loricurie.files.wordpress.com/2010/11/seattle-skyline.jpg";
#else
                CurrentTrip.MainPhotoUrl = await BingHelper.QueryBingImages("Seattle", CurrentPosition.Latitude, CurrentPosition.Longitude);
#endif
                CurrentTrip.Rating = 90;
                CurrentTrip.TimeStamp = DateTime.UtcNow;
                if(string.IsNullOrWhiteSpace(CurrentTrip.TripId))
                    CurrentTrip.TripId = DateTime.Now.ToString("d") + DateTime.Now.ToString("t");



                await StoreManager.TripStore.InsertAsync(CurrentTrip);

                foreach (var photo in photos)
                {
                    photo.TripId = CurrentTrip.Id;
                    await StoreManager.PhotoStore.InsertAsync(photo);
                }

                CurrentTrip = new Trip();
                CurrentTrip.Trail = new ObservableRangeCollection<Trail>();
                photos = new List<Photo>();
                OnPropertyChanged(nameof(CurrentTrip));

                return true;
            }
            catch(Exception ex)
            {
                Logger.Instance.Report(ex);
            }
            finally
            {
                track.Stop();
                IsBusy = false;
                progress?.Hide();
                progress?.Dispose();
            }

            return false;
        }


		ICommand  startTrackingTripCommand;
		public ICommand StartTrackingTripCommand =>
		    startTrackingTripCommand ?? (startTrackingTripCommand = new RelayCommand(async () => await ExecuteStartTrackingTripCommandAsync())); 

		public async Task ExecuteStartTrackingTripCommandAsync ()
		{
			if(IsBusy)
				return;

			try 
			{

				if (Geolocator.IsGeolocationAvailable && Geolocator.IsGeolocationEnabled)
				{
					Geolocator.AllowsBackgroundUpdates = true;
					Geolocator.DesiredAccuracy = 25;

                    Geolocator.PositionChanged += Geolocator_PositionChanged;
                    await Geolocator.StartListeningAsync(1, 1);
				}
				else
				{

                    if (CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.Android ||
                    CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.iOS)
                    {
                        Acr.UserDialogs.UserDialogs.Instance.Alert("Please ensure that geolocation is enabled and permissions are allowed for MyTrips to start a recording.",
                                                                   "Geolcoation Disabled", "OK");
                    }
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

		ICommand  stopTrackingTripCommand;
		public ICommand StopTrackingTripCommand =>
		stopTrackingTripCommand ?? (stopTrackingTripCommand = new RelayCommand(async () => await ExecuteStopTrackingTripCommandAsync())); 

		public async Task ExecuteStopTrackingTripCommandAsync ()
		{
            if(IsBusy || !IsRecording)
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

				var trail = new Trail
				{
					TimeStamp = DateTime.UtcNow,
					Latitude = userLocation.Latitude,
					Longitude = userLocation.Longitude,
				};


				CurrentTrip.Trail.Add (trail);

                if (CurrentTrip.Trail.Count > 1)
                {
                    var previous = CurrentTrip.Trail[CurrentTrip.Trail.Count - 2];//2 back now
                    CurrentTrip.Distance += DistanceUtils.CalculateDistance(userLocation.Latitude, userLocation.Longitude, previous.Latitude, previous.Longitude);
                    OnPropertyChanged(nameof(CurrentTrip.Distance));
                }
                var timeDif = trail.TimeStamp - CurrentTrip.TimeStamp;
                //track minutes first and then calculat the hours
                if(timeDif.TotalHours > 0)
                    ElapsedTime = $"{timeDif.Minutes}m";
                else
                    ElapsedTime = $"{(int)timeDif.TotalHours}h {timeDif.Minutes}m";
			}

            CurrentPosition = e.Position;
		}

        ICommand  takePhotoCommand;
        public ICommand TakePhotoCommand =>
            takePhotoCommand ?? (takePhotoCommand = new RelayCommand(async () => await ExecuteTakePhotoCommandAsync())); 

        async Task ExecuteTakePhotoCommandAsync()
        {
            try 
            {
                
                await Media.Initialize();

                if (!Media.IsCameraAvailable || !Media.IsTakePhotoSupported)
                {


                    if (CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.Android ||
                    CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.iOS)
                    {
                        Acr.UserDialogs.UserDialogs.Instance.Alert("Please ensure that camera is enabled and permissions are allowed for MyTrips to take photos.",
                                                                   "Camera Disabled", "OK");
                    }
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

                if(photo == null)
                {
                    return;
                }

                Acr.UserDialogs.UserDialogs.Instance.Toast(new Acr.UserDialogs.ToastConfig(Acr.UserDialogs.ToastEvent.Success, "Photo taken!") 
                { 
                    Duration = TimeSpan.FromSeconds(3), 
                    TextColor = System.Drawing.Color.White, 
                    BackgroundColor = System.Drawing.Color.FromArgb(96, 125, 139) 
                });

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
