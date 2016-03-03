using MvvmHelpers;
using MyTrips.Utils;

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


        int totalMiles;
        public int TotalMiles
        {
            get { return totalMiles; }
            set { SetProperty(ref totalMiles, value); }
        }

        int totalTime;
        public int TotalTime
        {
            get { return totalTime; }
            set { SetProperty(ref totalTime, value); }
        }


        int avgSpeedLocal;
        public int AvgSpeedLocal
        {
            get { return avgSpeedLocal; }
            set { SetProperty(ref avgSpeedLocal, value); }
        }


        int avgSpeedHighway;
        public int AvgSpeedHighway
        {
            get { return avgSpeedHighway; }
            set { SetProperty(ref avgSpeedHighway, value); }
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
        public ObservableRangeCollection<Tip> Tips { get; set; }

   

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
                new DrivingSkillsBucket()   {   betterThan=25,    description="Pretty bad..",   color=SkillsColor.Orange },
                new DrivingSkillsBucket()   {   betterThan=50,    description="OK",   color=SkillsColor.Yellow },
                new DrivingSkillsBucket()   {   betterThan=75,    description="Great!",   color=SkillsColor.Green }
            };
        }

        void UpdatePlacementBucket(int drivingSkills)
        {
            for(int i= drivingSkillsBuckets-1; i>=0; i--)
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
