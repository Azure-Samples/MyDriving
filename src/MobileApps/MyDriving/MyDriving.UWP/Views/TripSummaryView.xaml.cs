// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;
using MyDriving.ViewModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyDriving.UWP.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TripSummaryView
    {
        public TripSummaryView()
        {
            InitializeComponent();
            ViewModel = new TripSummaryViewModel();

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

            HardAccelerationsTab.Title1 = "Hard";
            HardAccelerationsTab.Title2 = "ACCELERATIONS";
        }

        public TripSummaryViewModel ViewModel { get; set; }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel = e.Parameter as TripSummaryViewModel;
            DataContext = this;
            UpdateSummary();
            // Enable back button behavior
            SystemNavigationManager systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            systemNavigationManager.BackRequested += SystemNavigationManager_BackRequested;

            //  DrawPath();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            SystemNavigationManager systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            systemNavigationManager.BackRequested -= SystemNavigationManager_BackRequested;
        }

        private void SystemNavigationManager_BackRequested(object sender, BackRequestedEventArgs e)
        {
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

        private async void UpdateSummary()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                //TotalDistanceTab.SetValue( ViewModel.TotalDistance);
            });
        }

        private void ButtonClick_CloseView(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            App.SetTitle("CURRENT TRIP");
            Frame.Navigate(typeof (CurrentTripView));
        }
    }
}