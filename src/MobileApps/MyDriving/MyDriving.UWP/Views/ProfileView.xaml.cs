// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;
using MyDriving.ViewModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyDriving.UWP.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProfileView
    {
        readonly ProfileViewModel profileViewModel;

        public ProfileView()
        {
            profileViewModel = new ProfileViewModel();
            DataContext = profileViewModel;
            InitializeComponent();

            TotalDistanceTab.Title1 = "Total";
            TotalDistanceTab.Title2 = "DISTANCE";

            TotalTimeTab.Title1 = "Total";
            TotalTimeTab.Title2 = "TIME";

            MaxSpeedTab.Title1 = "Max";
            MaxSpeedTab.Title2 = "SPEED";

            FuelConsumptionTab.Title1 = "Total";
            FuelConsumptionTab.Title2 = "FUEL USED";

            HardBreaksTab.Title1 = "Hard";
            HardBreaksTab.Title2 = "STOPS";

            HardAccelTab.Title1 = "Hard";
            HardAccelTab.Title2 = "ACCELERATIONS";

            TotalTripsTab.Title1 = "Total";
            TotalTripsTab.Title2 = "TRIPS";
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            App.SetTitle("PROFILE");
            // Enable back button behavior
            SystemNavigationManager systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            systemNavigationManager.BackRequested += SystemNavigationManager_BackRequested;
            profileViewModel.PropertyChanged += ViewModel_PropertyChanged;
            await profileViewModel.UpdateProfileAsync();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            SystemNavigationManager systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            systemNavigationManager.BackRequested -= SystemNavigationManager_BackRequested;
        }

        private void SystemNavigationManager_BackRequested(object sender, BackRequestedEventArgs e)
        {
            profileViewModel.PropertyChanged += ViewModel_PropertyChanged;
            if (!e.Handled)
            {
                e.Handled = TryGoBack();
            }
        }

        private bool TryGoBack()
        {
            bool navigated = false;
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
                navigated = true;
            }
            return navigated;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (string.CompareOrdinal(e.PropertyName, nameof(profileViewModel.DrivingSkills)) == 0)
            {
                CirclePercentage.Percentage = (double)profileViewModel.DrivingSkills;
                CirclePercentage.Update();
            }
        }
    }
}