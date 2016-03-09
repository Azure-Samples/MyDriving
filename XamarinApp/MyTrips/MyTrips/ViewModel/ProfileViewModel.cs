using Microsoft.WindowsAzure.MobileServices;
using MvvmHelpers;
using MyTrips.AzureClient;
using MyTrips.Utils;
using System.Threading.Tasks;
using System.Linq;
using System;


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

        public async Task<bool> UpdateProfileAsync()
        {
            if (IsBusy)
                return false;

            var progress = Acr.UserDialogs.UserDialogs.Instance.Loading("Loading profile...", maskType: Acr.UserDialogs.MaskType.Clear);
            var error = false;
            try
            {
                IsBusy = true;

                var users = await StoreManager.UserStore.GetItemsAsync(0, 100, true);

                var currentUser = users.FirstOrDefault(s => s.UserId == Settings.UserId);

                if (currentUser == null)
                {
                    error = true;
                }
                else
                {
                    TotalDistance = currentUser.TotalDistance;
                }
                //update stats here.
            }
            catch (Exception ex)
            {
                Logger.Instance.Report(ex);
                error = true;
            }
            finally
            {
                progress?.Dispose();
                IsBusy = false;
            }

            if (error)
            {
                //display dialog
            }

            return !error;
        }


        DrivingSkillsBucket drivingSkillsPlacementBucket;
        public DrivingSkillsBucket DrivingSkillsPlacementBucket
        {
            get { return drivingSkillsPlacementBucket; }
            set { SetProperty(ref drivingSkillsPlacementBucket, value); }
        }

        public string TotalDistanceUnits
        {
            get {
                var units = Settings.MetricUnits ? "kilometers" : "miles";
                return $"{TotalDistance} {units}";
            }
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

        public string AverageSpeedUnits
        {
            get {
                var units = Settings.MetricUnits ? "km/h" : "mph";
                return $"{AvgSpeed} {units}";
            }
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



        async Task UpdatePictureAsync()
        {
            IMobileServiceClient client = ServiceLocator.Instance.Resolve<IAzureClient>()?.Client;
            await Helpers.UserProfileHelper.GetUserProfileAsync(client);
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
                new DrivingSkillsBucket()   {  betterThan=0,  description="Very bad",  color=SkillsColor.Red },
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
