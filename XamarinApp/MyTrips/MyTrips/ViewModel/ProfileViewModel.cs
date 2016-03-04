using Microsoft.WindowsAzure.MobileServices;
using MvvmHelpers;
using MyTrips.AzureClient;
using MyTrips.Utils;
using System.Threading.Tasks;


namespace MyTrips.ViewModel
{
    public class ProfileViewModel : ViewModelBase
    {
        public ProfileViewModel()
        {
            InitializeDrivingSkills();
        }

        const int drivingSkillsBuckets = 4;
        int drivingSkills;   //percentage
        public int DrivingSkills
        {
            get { return drivingSkills; }
            set
            {
                SetProperty(ref drivingSkills, value);
                UpdatePlacementBucket(drivingSkills);
            }
        }

        DrivingSkillsBucket drivingSkillsPlacementBucket;
        public DrivingSkillsBucket DrivingSkillsPlacementBucket
        {
            get { return drivingSkillsPlacementBucket; }
            set { SetProperty(ref drivingSkillsPlacementBucket, value); }
        }


        double totalDistance;
        public double TotalDistance
        {
            get { return totalDistance; }
            set { SetProperty(ref totalDistance, value); }
        }

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

        public class Tip
        {

        }
        ObservableRangeCollection<Tip> tips = new ObservableRangeCollection<Tip>();
        public ObservableRangeCollection<Tip> Tips
        {
            get { return tips; }
            set { SetProperty(ref tips, value); }
        }


        UserPictureSourceKind pictureSourceKind = Settings.Current.UserPictureSourceKind;
        public UserPictureSourceKind UserPictureSourceKind
        {
            get { return pictureSourceKind; }
            set { SetProperty(ref pictureSourceKind, value); }
        }

        async Task UpdatePictureAsync()
        {
            IMobileServiceClient client = ServiceLocator.Instance.Resolve<IAzureClient>()?.Client;
            await Helpers.UserProfileHelper.GetUserProfileAsync(client);
            UserPictureSourceKind = Settings.Current.UserPictureSourceKind;
        }


        private DrivingSkillsBucket[] Skills
        {
            get; set;
        }

        void InitializeDrivingSkills()
        {
            // to do find specifications for colors/desription 
            Skills = new DrivingSkillsBucket[drivingSkillsBuckets]
            {
                new DrivingSkillsBucket()    {  betterThan=0,  description="Very bad",  color=SkillsColor.Red },
                new DrivingSkillsBucket()   {   betterThan=25,    description="Poor",   color=SkillsColor.Orange },
                new DrivingSkillsBucket()   {   betterThan=50,    description="Good",   color=SkillsColor.Yellow },
                new DrivingSkillsBucket()   {   betterThan=75,    description="Great!",   color=SkillsColor.Green }
            };
        }

        void UpdatePlacementBucket(int drivingSkills)
        {
            for (int i = drivingSkillsBuckets - 1; i >= 0; i--)
            {
                if (drivingSkills > Skills[i].betterThan)
                {
                    DrivingSkillsPlacementBucket = Skills[i];
                    return;
                }
            }
        }
    }

    public struct DrivingSkillsBucket
    {
        public int betterThan;  //percent
        public string description;
        public SkillsColor color;
    }

    public enum SkillsColor
    {
        Red = 0,
        Orange,
        Yellow,
        Green
    }
}
