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

namespace MyTrips.ViewModel
{
    public class CurrentTripViewModel : ViewModelBase
    {
		public Trip CurrentTrip { get; set; }

		public CurrentTripViewModel()
		{
			CurrentTrip = new Trip();
            CurrentTrip.Trail = new List<Trail>
            {
                    new Trail()
            };
		}

		public IGeolocator Geolocator { get; } = CrossGeolocator.Current;

		ICommand  startTrackingTripCommand;
		public ICommand StartTrackingTrip =>
		startTrackingTripCommand ?? (startTrackingTripCommand = new RelayCommand(async () => await ExecuteStartTrackingTripCommandAsync())); 

		public async Task ExecuteStartTrackingTripCommandAsync ()
		{
			if(IsBusy)
				return;

			try 
			{
				IsBusy = true;

				if (Geolocator.IsGeolocationAvailable && Geolocator.IsGeolocationEnabled)
				{
					Geolocator.AllowsBackgroundUpdates = true;
					Geolocator.DesiredAccuracy = 25;

                    var startingPostion = await Geolocator.GetPositionAsync (timeoutMilliseconds: 2500);
                    var trail = new Trail
                        {
                            TimeStamp = DateTime.UtcNow,
                            Latitude = startingPostion.Latitude,
                            Longitude = startingPostion.Longitude,
                            
                        };
					// Trip start time
                    CurrentTrip.Trail[0].TimeStamp = DateTime.UtcNow;

                    
					
                    CurrentTrip.Trail.Add(trail);

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
				IsBusy = false;
			}
		}

		ICommand  stopTrackingTripCommand;
		public ICommand StopTrackingTrip =>
		stopTrackingTripCommand ?? (stopTrackingTripCommand = new RelayCommand(async () => await ExecuteStopTrackingTripCommandAsync())); 

		public async Task ExecuteStopTrackingTripCommandAsync ()
		{
			if(IsBusy)
				return;

			try 
			{
				IsBusy = true;

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
				IsBusy = false;
			}
		}

		void Geolocator_PositionChanged(object sender, PositionEventArgs e)
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
	}
}
