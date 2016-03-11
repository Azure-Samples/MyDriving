using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using MyTrips.DataObjects;
using MyTrips.Helpers;
using MyTrips.Utils;

using Acr.UserDialogs;
using MvvmHelpers;
using Plugin.DeviceInfo;

namespace MyTrips.ViewModel
{
    public class PastTripsViewModel : ViewModelBase
    {
        public ObservableRangeCollection<Trip> Trips { get; } = new ObservableRangeCollection<Trip>();

        ICommand  loadPastTripsCommand;

        public ICommand LoadPastTripsCommand =>
        loadPastTripsCommand ?? (loadPastTripsCommand = new RelayCommand(async () => await ExecuteLoadPastTripsCommandAsync())); 

        public async Task ExecuteLoadPastTripsCommandAsync()
        {
            if(IsBusy)
                return;

            var track = Logger.Instance.TrackTime("LoadTrips");
            track.Start();

			IProgressDialog progressDialog = null;
			
			progressDialog = UserDialogs.Instance.Loading("Loading trips...", maskType: MaskType.Clear);
			   

            try 
            {
                IsBusy = true;
                CanLoadMore = true;

                Trips.ReplaceRange(await StoreManager.TripStore.GetItemsAsync(0, 25, true));

                CanLoadMore = Trips.Count == 25;
            }
            catch (Exception ex) 
            {
                Logger.Instance.Report(ex);
            } 
            finally 
            {
                track.Stop();
                IsBusy = false;

				progressDialog?.Dispose();
            }
        }

        ICommand  loadMorePastTripsCommand;
        public ICommand LoadMorePastTripCommand =>
        loadMorePastTripsCommand ?? (loadMorePastTripsCommand = new RelayCommand(async () => await ExecuteLoadMorePastTripsCommandAsync()));

        public async Task ExecuteLoadMorePastTripsCommandAsync()
        {
            if (IsBusy || !CanLoadMore)
                return;

            var track = Logger.Instance.TrackTime("LoadMoreTrips");
            track.Start();
            var progress = Acr.UserDialogs.UserDialogs.Instance.Loading("Loading more trips...", maskType: Acr.UserDialogs.MaskType.Clear);
            
            try
            {
                IsBusy = true;
                CanLoadMore = true;

                Trips.AddRange(await StoreManager.TripStore.GetItemsAsync(Trips.Count, 25, true));
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
        }

    }
}