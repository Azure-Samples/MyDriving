// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using MyDriving.Utils;
using MyDriving.ViewModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyDriving.UWP.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginView : Page
    {
        readonly LoginViewModel _viewModel;

        public LoginView()
        {
            InitializeComponent();
            DataContext = _viewModel = new LoginViewModel();
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_viewModel.IsLoggedIn):
                    ShowUserWelcome();
                    break;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        //This button is temporary - intended to make it easier to debug app
        private void SkipAuthBtn_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.InitFakeUser();
            Frame.NavigationFailed += OnNavigationFailed;
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

            Frame.Navigate(typeof (CurrentTripView));
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            //For now, don't let user go back to the log in page; need to finalize what this experience should be like when user keeps pushing back
            if (Frame.CurrentSourcePageType != typeof (PastTripsMenuView))
            {
                if (Frame != null && Frame.CanGoBack)
                {
                    e.Handled = true;
                    Frame.GoBack();
                }
            }
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void ShowUserWelcome()
        {
            if (!string.IsNullOrWhiteSpace(Settings.Current.AzureMobileUserId))
            {
                LoginButtons.Visibility = Visibility.Collapsed;
                AppLogo.Visibility = Visibility.Collapsed;
                WelcomeText.Text = "Welcome " + Settings.Current.UserFirstName + "!";
                WelcomeText.Visibility = Visibility.Visible;
                SetImageSource();
                ProfileImage.Visibility = Visibility.Visible;
                ContinueButton.Visibility = Visibility.Visible;
            }
            else //if no user info to show, go directly to next page
            {
                Frame.Navigate(typeof (CurrentTripView));
            }
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (CurrentTripView));
        }

        private void SetImageSource()
        {
            ProfileImage.Source = new BitmapImage(new Uri(Settings.Current.UserProfileUrl));
        }
    }
}