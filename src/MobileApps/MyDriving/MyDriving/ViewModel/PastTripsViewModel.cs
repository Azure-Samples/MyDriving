// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MyDriving.DataObjects;
using MyDriving.Helpers;
using MyDriving.Utils;
using Acr.UserDialogs;
using MvvmHelpers;
using MyDriving.Utils.Helpers;

namespace MyDriving.ViewModel
{
    public class PastTripsViewModel : ViewModelBase
    {
        ICommand loadMorePastTripsCommand;

        ICommand loadPastTripsCommand;
        public ObservableRangeCollection<Trip> Trips { get; } = new ObservableRangeCollection<Trip>();

        public ICommand LoadPastTripsCommand =>
            loadPastTripsCommand ??
            (loadPastTripsCommand = new RelayCommand(async () => await ExecuteLoadPastTripsCommandAsync()));

        public ICommand LoadMorePastTripCommand =>
            loadMorePastTripsCommand ??
            (loadMorePastTripsCommand = new RelayCommand(async () => await ExecuteLoadMorePastTripsCommandAsync()));

        public async Task<bool> ExecuteDeleteTripCommand(Trip trip)
        {
            if (IsBusy)
                return false;

            Logger.Instance.Track("DeleteTrip");
            var progress = UserDialogs.Instance.Loading("Deleting Trip...", show: false, maskType: MaskType.Clear);

            try
            {
                var result =
                    await
                        UserDialogs.Instance.ConfirmAsync($"Are you sure you want to delete trip: {trip.Name}?",
                            "Delete trip?", "Delete", "Cancel");

                if (!result)
                    return false;

                progress?.Show();

                await StoreManager.TripStore.RemoveAsync(trip);

                Trips.Remove(trip);
            }
            catch (Exception ex)
            {
                Logger.Instance.Report(ex);
            }
            finally
            {
                progress?.Dispose();
            }

            return true;
        }

        public async Task ExecuteLoadPastTripsCommandAsync()
        {
            if (IsBusy)
                return;

            ProgressDialogManager.LoadProgressDialog("Loading trips...");

            try
            {
                IsBusy = true;
                CanLoadMore = true;

                var items = await StoreManager.TripStore.GetItemsAsync(0, 25, true);
                Trips.ReplaceRange(items);

                CanLoadMore = Trips.Count == 25;
                Logger.Instance.Track("LoadTrips");
            }
            catch (Exception ex)
            {
                Logger.Instance.Report(ex);
                Logger.Instance.Track("Loading trips failed");
            }
            finally
            {
                IsBusy = false;

                ProgressDialogManager.DisposeProgressDialog();
            }

            if (Trips.Count == 0)
            {
                UserDialogs.Instance.Alert(
                    "Looks like you don't have any trips recorded yet, feel free to start one up.",
                    "No Past Trips", "OK");
            }
        }

        public async Task ExecuteLoadMorePastTripsCommandAsync()
        {
            if (IsBusy || !CanLoadMore)
                return;

            var track = Logger.Instance.TrackTime("LoadMoreTrips");
            track?.Start();
            ProgressDialogManager.LoadProgressDialog("Loading more trips...");

            try
            {
                IsBusy = true;
                CanLoadMore = true;

                var newCount = Trips.Count + 25;
                Trips.AddRange(await StoreManager.TripStore.GetItemsAsync(Trips.Count, 25, true));

                CanLoadMore = Trips.Count == newCount;
            }
            catch (Exception ex)
            {
                Logger.Instance.Report(ex);
            }
            finally
            {
                track?.Stop();
                IsBusy = false;
                ProgressDialogManager.DisposeProgressDialog();
            }
        }
    }
}