// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using MyDriving.UWP.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyDriving.UWP.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SplitViewShell
    {
        SplitViewButtonContent selectedControl;

        public SplitViewShell(Frame frame)
        {
            InitializeComponent();
            MyDrivingSplitView.Content = frame;
            frame.Navigated += Frame_Navigated;

            Current.LabelText = "Current";
            Current.DefaultImageSource =
                new BitmapImage(new Uri("ms-appx:///Assets/SplitView/default_current.png", UriKind.Absolute));
            Current.SelectedImageSource =
                new BitmapImage(new Uri("ms-appx:///Assets/SplitView/selected_current.png", UriKind.Absolute));

            PastTrips.LabelText = "Past";
            PastTrips.DefaultImageSource =
                new BitmapImage(new Uri("ms-appx:///Assets/SplitView/default_pastTrips.png", UriKind.Absolute));
            PastTrips.SelectedImageSource =
                new BitmapImage(new Uri("ms-appx:///Assets/SplitView/selected_pastTrips.png", UriKind.Absolute));

            Profile.LabelText = "Profile";
            Profile.DefaultImageSource =
                new BitmapImage(new Uri("ms-appx:///Assets/SplitView/default_profile.png", UriKind.Absolute));
            Profile.SelectedImageSource =
                new BitmapImage(new Uri("ms-appx:///Assets/SplitView/selected_profile.png", UriKind.Absolute));

            Settings.LabelText = "Settings";
            Settings.DefaultImageSource =
                new BitmapImage(new Uri("ms-appx:///Assets/SplitView/default_settings.png", UriKind.Absolute));
            Settings.SelectedImageSource =
                new BitmapImage(new Uri("ms-appx:///Assets/SplitView/selected_settings.png", UriKind.Absolute));
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MyDrivingSplitView.IsPaneOpen = !MyDrivingSplitView.IsPaneOpen;
        }


        private void TripsButton_Click(object sender, RoutedEventArgs e)
        {
            SelectControl(PastTrips);
            MyDrivingSplitView.IsPaneOpen = false;
            PageTitle.Text = "PAST TRIPS";
            ((Frame) MyDrivingSplitView.Content).Navigate(typeof (PastTripsMenuView));
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            SelectControl(Profile);
            MyDrivingSplitView.IsPaneOpen = false;
            PageTitle.Text = "PROFILE";
            ((Frame) MyDrivingSplitView.Content).Navigate(typeof (ProfileView));
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SelectControl(Settings);
            MyDrivingSplitView.IsPaneOpen = false;
            PageTitle.Text = "SETTINGS";
            ((Frame) MyDrivingSplitView.Content).Navigate(typeof (SettingsView));
        }

        private void NewTripButton_Click(object sender, RoutedEventArgs e)
        {
            SelectControl(Current);
            MyDrivingSplitView.IsPaneOpen = false;
            PageTitle.Text = "CURRENT TRIP";
            ((Frame) MyDrivingSplitView.Content).Navigate(typeof (CurrentTripView));
        }


        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            MyDrivingSplitView.IsPaneOpen = false;

            var frame = MyDrivingSplitView.Content as Frame;

            var name = ((Page) frame.Content).Name;
            if (string.Compare(name, "Login", StringComparison.OrdinalIgnoreCase) == 0)
            {
                SetVisible(false);
            }
            else
            {
                SetVisible(true);
                if (string.IsNullOrWhiteSpace(PageTitle.Text))
                    PageTitle.Text = name;
            }
        }

        private void SelectControl(SplitViewButtonContent control)
        {
            selectedControl?.SetSelected(false);
            control.SetSelected(true);
            selectedControl = control;
        }

        public void SetVisible(bool visible)
        {
            if (visible)
            {
                HamburgerGrid.Visibility = Visibility.Visible;
                SplitViewPanel.Visibility = Visibility.Visible;
                TitleGrid.Visibility = Visibility.Visible;
                HamburgerButton.IsEnabled = true;
                NewTripButton.IsEnabled = true;
                TripsButton.IsEnabled = true;
                ProfileButton.IsEnabled = true;
                SettingsButton.IsEnabled = true;
            }
            else
            {
                HamburgerGrid.Visibility = Visibility.Collapsed;
                SplitViewPanel.Visibility = Visibility.Collapsed;
                TitleGrid.Visibility = Visibility.Collapsed;
                HamburgerButton.IsEnabled = false;
                NewTripButton.IsEnabled = false;
                TripsButton.IsEnabled = false;
                ProfileButton.IsEnabled = false;
                SettingsButton.IsEnabled = false;
                PageTitle.Text = "";
            }
        }

        public void SetTitle(string title)
        {
            PageTitle.Text = title;
        }
    }
}