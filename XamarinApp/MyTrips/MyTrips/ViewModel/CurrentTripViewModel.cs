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

using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using MvvmHelpers;
using Plugin.Media.Abstractions;
using Plugin.Media;

namespace MyTrips.ViewModel
{
    public class CurrentTripViewModel : ViewModelBase
    {
        public Trip CurrentTrip { get; private set; }

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

		public CurrentTripViewModel()
		{
			CurrentTrip = new Trip();
            CurrentTrip.Trail = new ObservableRangeCollection<Trail>();
            CurrentTrip.Photos = new ObservableRangeCollection<Photo>();
		}

		public IGeolocator Geolocator => CrossGeolocator.Current;

        public IMedia Media => CrossMedia.Current;


        ICommand  startRecordingTripCommand;
        public ICommand StartRecordingTripCommand =>
        startRecordingTripCommand ?? (startRecordingTripCommand = new RelayCommand(async () => await ExecuteStartRecordingTripCommandAsync()));

        public async Task ExecuteStartRecordingTripCommandAsync()
        {
            if (IsBusy || IsRecording)
                return;

            try
            {
                IsRecording = true;
            }
            catch
            {
            }
            finally
            {
            }
        }

        ICommand  stopRecordingTripCommand;
        public ICommand StopRecordingTripCommand =>
        stopRecordingTripCommand ?? (stopRecordingTripCommand = new RelayCommand(async () => await ExecuteStopRecordingTripCommandAsync()));

        public async Task ExecuteStopRecordingTripCommandAsync()
        {
            if (IsBusy || !IsRecording)
                return;

            try
            {
                IsRecording = false;

                IsBusy = true;
                #if DEBUG
                await Task.Delay(5000);
                #endif

                //TODO: Insert into database
                CurrentTrip = new Trip();
                CurrentTrip.Trail = new ObservableRangeCollection<Trail>();
                CurrentTrip.Photos = new ObservableRangeCollection<Photo>();
                OnPropertyChanged(nameof(CurrentTrip));
            }
            catch
            {
            }
            finally
            {
                IsBusy = false;
            }
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
					// TODO: Show an alert letting them know about permissions via Messaging Center?
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
			if(IsBusy)
				return;

			try 
			{

				if (Geolocator.IsGeolocationAvailable && Geolocator.IsGeolocationEnabled)
				{
					Geolocator.PositionChanged -= Geolocator_PositionChanged;
					await Geolocator.StopListeningAsync();
				}
				else
				{
					// TODO: Show an alert letting them know about permissions via Messaging Center?
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

                if(!Media.IsCameraAvailable || !Media.IsTakePhotoSupported)
                    return;

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


                var local = await locationTask;
                var photoDB = new Photo
                    {
                        PhotoUrl = photo.Path,
                        Latitude = local.Latitude,
                        Longitude = local.Longitude, 
                        TimeStamp = DateTime.UtcNow,
                        TripId = CurrentTrip.Id
                    };

                //TODO: 
                CurrentTrip.Photos.Add(photoDB);

                photo.Dispose();
                
            } 
            catch (Exception ex) 
            {
                Logger.Instance.Report(ex);
            }
        }
	}
}
