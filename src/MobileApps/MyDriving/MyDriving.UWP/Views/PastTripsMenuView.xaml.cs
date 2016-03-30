// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MyDriving.DataObjects;
using MyDriving.ViewModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MyDriving.UWP.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PastTripsMenuView
    {
        public PastTripsMenuView()
        {
            InitializeComponent();
            ViewModel = new PastTripsViewModel();
        }

        public PastTripsViewModel ViewModel { get; set; }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            App.SetTitle("PAST TRIPS");
            // Enable back button behavior
            SystemNavigationManager systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            systemNavigationManager.BackRequested += SystemNavigationManager_BackRequested;

            await ViewModel.ExecuteLoadPastTripsCommandAsync();
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

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var trip = (Trip) e.AddedItems[0];
            Frame.Navigate(typeof (PastTripMapView), trip); // PastTripMapView does not exist
        }

        private void ListViewItem_Holding(object sender, HoldingRoutedEventArgs e)
        {
            var senderElement = sender as FrameworkElement;
            var flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);

            flyoutBase.ShowAt(senderElement);
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var trip = (e.OriginalSource as FrameworkElement).DataContext as Trip;

            if (trip == null)
                return;

            await ViewModel.ExecuteDeleteTripCommand(trip);

        }
        
        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.ExecuteLoadPastTripsCommandAsync();
        }
    }
}