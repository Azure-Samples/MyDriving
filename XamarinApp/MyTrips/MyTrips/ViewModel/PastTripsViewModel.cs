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
            Acr.UserDialogs.IProgressDialog progress = null;
            

            if (CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.Android ||
                CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.iOS)
            {
                progress = Acr.UserDialogs.UserDialogs.Instance.Progress("Loading trips...", show: false, maskType: Acr.UserDialogs.MaskType.Clear);
                progress.IsDeterministic = false;
                progress.Show();
            }
            try 
            {
                IsBusy = true;
                CanLoadMore = true;

#if DEBUG
                await Task.Delay(1000);
#endif

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
                progress?.Hide();
                progress?.Dispose();
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
            var progress = Acr.UserDialogs.UserDialogs.Instance.Progress("Loading more trips...", show: false,  maskType: Acr.UserDialogs.MaskType.Clear);
            progress.IsDeterministic = false;
            try
            {
                IsBusy = true;
                CanLoadMore = true;

                progress.Show();
#if DEBUG
                await Task.Delay(1000);
#endif
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
                progress.Hide();
                progress.Dispose();
            }
        }

    }
}
