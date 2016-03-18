// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.WindowsAzure.MobileServices;
using MvvmHelpers;
using MyDriving.AzureClient;
using MyDriving.Utils;
using System.Threading.Tasks;
using System.Linq;
using System;
using MyDriving.DataObjects;

namespace MyDriving.ViewModel
{
    public class ProfileViewModel : ViewModelBase
    {
        const int drivingSkillsBuckets = 4;
        int drivingSkills; //percentage


        DrivingSkillsBucket drivingSkillsPlacementBucket;

        double fuelUsed;

        long hardAccelerations;

        long hardStops;

        double maxSpeed;

        double totalDistance;

        double totalTime;

        long totalTrips;

        public ProfileViewModel()
        {
            InitializeDrivingSkills();
        }

        public int DrivingSkills
        {
            get { return drivingSkills; }
            set
            {
                SetProperty(ref drivingSkills, value);
                UpdatePlacementBucket(drivingSkills);
            }
        }

        public DrivingSkillsBucket DrivingSkillsPlacementBucket
        {
            get { return drivingSkillsPlacementBucket; }
            set { SetProperty(ref drivingSkillsPlacementBucket, value); }
        }

        public string FuelUnits => Settings.MetricUnits ? "L" : "gal.";

        public double FuelConverted => Settings.MetricUnits ? FuelUsed/.264172 : FuelUsed;

        public string FuelDisplayNoUnits => FuelConverted.ToString("F");

        public string FuelDisplay => $"{FuelDisplayNoUnits} {FuelUnits.ToLowerInvariant()}";

        public string DistanceUnits => Settings.MetricDistance ? "km" : "miles";

        public string TotalDistanceDisplayNoUnits => DistanceConverted.ToString("F");

        public string TotalDistanceDisplay => $"{TotalDistanceDisplayNoUnits} {DistanceUnits}";

        public double DistanceConverted => (Settings.Current.MetricDistance ? (TotalDistance*1.60934) : TotalDistance);

        public string SpeedUnits => Settings.MetricDistance ? "km/h" : "mph";

        public double MaxSpeedConverted => Settings.MetricDistance ? MaxSpeed : MaxSpeed*0.621371;

        public string MaxSpeedDisplayNoUnits => MaxSpeedConverted.ToString("F");

        public string MaxSpeedDisplay => $"{MaxSpeedDisplayNoUnits} {SpeedUnits}";

        public string TotalTimeDisplay
        {
            get
            {
                var time = TimeSpan.FromSeconds(TotalTime);
                if (time.TotalMinutes < 1)
                    return $"{time.Seconds}s";

                if (time.TotalHours < 1)
                    return $"{time.Minutes}m {time.Seconds}s";

                return $"{(int) time.TotalHours}h {time.Minutes}m {time.Seconds}s";
            }
        }

        public double TotalDistance
        {
            get { return totalDistance; }
            set
            {
                if (!SetProperty(ref totalDistance, value))
                    return;

                OnPropertyChanged(nameof(DistanceUnits));
                OnPropertyChanged(nameof(TotalDistanceDisplay));
                OnPropertyChanged(nameof(TotalDistanceDisplayNoUnits));
                OnPropertyChanged(nameof(DistanceConverted));
            }
        }

        public double FuelUsed
        {
            get { return fuelUsed; }
            set
            {
                if (!SetProperty(ref fuelUsed, value))
                    return;

                OnPropertyChanged(nameof(FuelUnits));
                OnPropertyChanged(nameof(FuelDisplay));
                OnPropertyChanged(nameof(FuelDisplayNoUnits));
                OnPropertyChanged(nameof(FuelConverted));
            }
        }

        public double TotalTime
        {
            get { return totalTime; }
            set
            {
                if (!SetProperty(ref totalTime, value))
                    return;

                OnPropertyChanged(nameof(TotalTimeDisplay));
            }
        }

        public double MaxSpeed
        {
            get { return maxSpeed; }
            set
            {
                if (!SetProperty(ref maxSpeed, value))
                    return;

                OnPropertyChanged(nameof(SpeedUnits));
                OnPropertyChanged(nameof(MaxSpeedConverted));
                OnPropertyChanged(nameof(MaxSpeedDisplayNoUnits));
                OnPropertyChanged(nameof(MaxSpeedDisplay));
            }
        }

        public long HardStops
        {
            get { return hardStops; }
            set { SetProperty(ref hardStops, value); }
        }

        public long HardAccelerations
        {
            get { return hardAccelerations; }
            set { SetProperty(ref hardAccelerations, value); }
        }

        public long TotalTrips
        {
            get { return totalTrips; }
            set { SetProperty(ref totalTrips, value); }
        }

        DrivingSkillsBucket[] Skills { get; set; }

        public async Task<bool> UpdateProfileAsync()
        {
            if (IsBusy)
                return false;

            var progress = Acr.UserDialogs.UserDialogs.Instance.Loading("Loading profile...",
                maskType: Acr.UserDialogs.MaskType.Clear);
            var error = false;
            try
            {
                IsBusy = true;

                var users = await StoreManager.UserStore.GetItemsAsync(0, 100, true);

                var currentUser = users.FirstOrDefault(s => s.UserId == Settings.UserUID);

                if (currentUser == null)
                {
                    error = true;
                }
                else
                {
                    TotalDistance = currentUser.TotalDistance;
                    HardStops = currentUser.HardStops;
                    HardAccelerations = currentUser.HardAccelerations;
                    DrivingSkills = currentUser.Rating;
                    TotalTime = currentUser.TotalTime;
                    TotalTrips = currentUser.TotalTrips;
                    FuelUsed = currentUser.FuelConsumption;
                    MaxSpeed = currentUser.MaxSpeed;
#if DEBUG || XTC
                    if (currentUser.Rating == 0)
                        DrivingSkills = 86;
#endif
                    OnPropertyChanged("Stats");
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

            return !error;
        }

        async Task UpdatePictureAsync()
        {
            IMobileServiceClient client = ServiceLocator.Instance.Resolve<IAzureClient>()?.Client;
            await Helpers.UserProfileHelper.GetUserProfileAsync(client);
        }

        void InitializeDrivingSkills()
        {
            // to do find specifications for colors/desription 
            Skills = new DrivingSkillsBucket[drivingSkillsBuckets]
            {
                new DrivingSkillsBucket() {BetterThan = 0, Description = "Poor"},
                new DrivingSkillsBucket() {BetterThan = 45, Description = "Average"},
                new DrivingSkillsBucket() {BetterThan = 75, Description = "Great!"},
                new DrivingSkillsBucket() {BetterThan = 90, Description = "Amazing!"}
            };
        }

        void UpdatePlacementBucket(int skills)
        {
            for (int i = drivingSkillsBuckets - 1; i >= 0; i--)
            {
                if (skills > Skills[i].BetterThan)
                {
                    DrivingSkillsPlacementBucket = Skills[i];
                    return;
                }
            }
        }
    }

    public struct DrivingSkillsBucket
    {
        public int BetterThan { get; set; }
        public string Description { get; set; }
    }
}