using System;
using MyTrips.DataObjects;
using System.Windows.Input;
using System.Threading.Tasks;
using MyTrips.Utils;
using MyTrips.Helpers;

namespace MyTrips.ViewModel
{
    public class TripDetailsViewModel : ViewModelBase
    {
        public string TripId { get; set; } = string.Empty;
        public Trip CurrentTrip {get;set;}
        ICommand  loadTripCommand;
        public ICommand LoadTripCommand =>
            loadTripCommand ?? (loadTripCommand = new RelayCommand(async () => await ExecuteLoadTripCommandAsync())); 

        public async Task ExecuteLoadTripCommandAsync()
        {
            if(IsBusy)
                return;

            try 
            {
                IsBusy = true;

                CurrentTrip = await StoreManager.TripStore.GetItemAsync(TripId);

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

