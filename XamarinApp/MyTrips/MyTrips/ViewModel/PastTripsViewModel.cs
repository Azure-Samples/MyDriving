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
using MyTrips.SampleData;

namespace MyTrips.ViewModel
{
    public class PastTripsViewModel : ViewModelBase
    {
        private ObservableCollection<Trip> pastTrips;

        public ObservableCollection<Trip> PastTrips { get { return this.pastTrips; } }

        public PastTripsViewModel()
        {
            var trips = TripSampleData.GetTrips();
            this.pastTrips = new ObservableCollection<Trip>(trips);
        }

        //TODO: commenting out for now...will follow up on this
        //ICommand  loadPastTripsCommand;
        //public ICommand LoadPastTripsCommand =>
        //    loadPastTripsCommand ?? (loadPastTripsCommand = new RelayCommand(async () => await ExecuteLoadPastTripsCommandAsync())); 

        //public async Task ExecuteLoadPastTripsCommandAsync()
        //{
        //    if(IsBusy)
        //        return;
            
        //    try 
        //    {
        //        IsBusy = true;

        //        Trips.ReplaceRange(await StoreManager.TripStore.GetItemsAsync());

        //    }
        //    catch (Exception ex) 
        //    {
        //        Logger.Instance.Report(ex);
        //    } 
        //    finally 
        //    {
        //        IsBusy = false;
        //    }
        //}
    }
}
