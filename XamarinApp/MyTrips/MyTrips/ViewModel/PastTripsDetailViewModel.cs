using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MyTrips.Helpers;
using MyTrips.Utils;
using MvvmHelpers;
using MyTrips.DataObjects;
using System.Collections.ObjectModel;
using Plugin.DeviceInfo;

namespace MyTrips.ViewModel
{
	public class PastTripsDetailViewModel : ViewModelBase
	{
		public Trip Trip { get; set; }

        public PastTripsDetailViewModel()
        {
        }

		public PastTripsDetailViewModel(Trip trip)
		{
			Title = trip.TripId;
			Trip = trip;
		}

        ICommand  loadTripCommand;
        public ICommand LoadTripCommand =>
        loadTripCommand ?? (loadTripCommand = new RelayCommand<string>(async (id) => await ExecuteLoadTripCommandAsync(id))); 

        public async Task ExecuteLoadTripCommandAsync(string id)
        {
            if(IsBusy)
                return;

            Acr.UserDialogs.IProgressDialog progress = null;


            if (CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.Android ||
                CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.iOS)
            {
                progress = Acr.UserDialogs.UserDialogs.Instance.Progress("Loading trip details...", show: false, maskType: Acr.UserDialogs.MaskType.Clear);
                progress.IsDeterministic = false;
            }
            try 
            {
                progress?.Show();
                IsBusy = true;

                Trip = await StoreManager.TripStore.GetItemAsync(id);
                Title = Trip.TripId;
            }
            catch (Exception ex) 
            {
                Logger.Instance.Report(ex);
            } 
            finally 
            {
                progress?.Hide();
                progress?.Dispose();
                IsBusy = false;
            }
        }
	}
}

