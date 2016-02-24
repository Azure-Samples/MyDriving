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
            
            try 
            {
                IsBusy = true;

                Trips.ReplaceRange(await StoreManager.TripStore.GetItemsAsync());

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
    }
}
