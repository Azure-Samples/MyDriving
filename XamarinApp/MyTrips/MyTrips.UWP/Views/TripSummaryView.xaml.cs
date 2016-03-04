using MyTrips.ViewModel;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyTrips.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TripSummaryView : Page
    {
        TripSummaryViewModel tripSummaryViewModel;
        public TripSummaryView()
        {
            this.InitializeComponent();
            tripSummaryViewModel = new TripSummaryViewModel();
            DataContext = tripSummaryViewModel;

            TotalDistanceTab.Title1 = "Total";
            TotalDistanceTab.Title2 = "DISTANCE";

            TotalTimeTab.Title1 = "Total";
            TotalTimeTab.Title2 = "TIME";

            AvgSpeedTab.Title1 = "Avg";
            AvgSpeedTab.Title2 = "SPEED";

            HardBreaksTab.Title1 = "Hard";
            HardBreaksTab.Title2 = "BREAKS";

            TipsTab.Title1 = "TIPS";

        }
    }
}
