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
		Trip CurrentTrip { get; set; }

		public CurrentTripViewModel()
		{
			CurrentTrip = new Trip();
			CurrentTrip.Points = new List<Point>();
		}

		public IGeolocator Geolocator { get; set; } = CrossGeolocator.Current;

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

					// Trip start time

					var startingPostion = await Geolocator.GetPositionAsync (timeoutMilliseconds: 2500);
					var point = new Point
					{
						Latitude = startingPostion.Latitude,
						Longitude = startingPostion.Longitude
					};

					CurrentTrip.Points.Add(point);

					// TODO: Subscribe to this in iOS & Android projects to update map. 
					Geolocator.PositionChanged += Geolocator_PositionChanged;

					await Geolocator.StartListeningAsync(1, 1);

					System.Diagnostics.Debug.WriteLine(Geolocator.IsListening);
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
					// TODO: Unsubscribe on iOS / Android to avoid memory leak.
					Geolocator.PositionChanged -= Geolocator_PositionChanged;

					await Geolocator.StopListeningAsync();

					// trip end time
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
			var point = new Point
			{
				Latitude = userLocation.Latitude,
				Longitude = userLocation.Longitude
			};

			CurrentTrip.Points.Add (point);
		}
	}
}
