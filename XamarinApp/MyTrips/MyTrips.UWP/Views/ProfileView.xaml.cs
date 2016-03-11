using MyTrips.ViewModel;
using Windows.UI.Xaml.Controls;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyTrips.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProfileView : Page
    {
        ProfileViewModel profileViewModel;
        public ProfileView()
        {      
            profileViewModel = new ProfileViewModel();
            DataContext = profileViewModel;
            this.InitializeComponent();

            //profileViewModel.DrivingSkills = new System.Random().Next(0, 100);
            profileViewModel.DrivingSkills = 86;

            TotalDistanceTab.Title1 = "Total";
            TotalDistanceTab.Title2 = "DISTANCE";

            TotalTimeTab.Title1 = "Total";
            TotalTimeTab.Title2 = "TIME";

            AvgSpeedTab.Title1 = "Avg";
            AvgSpeedTab.Title2 = "SPEED";

            HardBreaksTab.Title1 = "Hard";
            HardBreaksTab.Title2 = "BREAKS";

            //TipsTab.Title1 = "TIPS";


        }
    }
}
