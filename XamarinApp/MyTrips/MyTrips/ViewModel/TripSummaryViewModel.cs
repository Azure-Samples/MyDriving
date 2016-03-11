using Microsoft.WindowsAzure.MobileServices;
using MvvmHelpers;
using MyTrips.AzureClient;
using MyTrips.Utils;
using System.Threading.Tasks;
using MyTrips.DataObjects;



namespace MyTrips.ViewModel
{
    public class TripSummaryViewModel : ViewModelBase
    {
        public Trip Trip { get; set; }

        double totalTime;
        public double TotalTime
        {
            get { return totalTime; }
            set { SetProperty(ref totalTime, value); }
        }

        double avgSpeed;
        public double AvgSpeed
        {
            get { return avgSpeed; }
            set { SetProperty(ref avgSpeed, value); }
        }

        int hardBreaks;
        public int HardBreaks
        {
            get { return hardBreaks; }
            set { SetProperty(ref hardBreaks, value); }
        }

        ObservableRangeCollection<Tip> tips = new ObservableRangeCollection<Tip>();
        public ObservableRangeCollection<Tip> Tips
        {
            get { return tips; }
            set { SetProperty(ref tips, value); }
        }

    }
}
