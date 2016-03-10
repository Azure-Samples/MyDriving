using Microsoft.WindowsAzure.MobileServices;
using MyTrips.Utils;
using MyTrips.ViewModel;
using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyTrips.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginView : Page
    {
        LoginViewModel viewModel;
        public LoginView()
        {
            this.InitializeComponent();
            DataContext = viewModel = new LoginViewModel();
            //Make sure you turn on azure in the ViewModelBase 
            
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(viewModel.IsLoggedIn):
                    ShowUserWelcome();
                    break;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        //This button is temporary - intended to make it easier to debug app
        private void SkipAuthBtn_Click(object sender, RoutedEventArgs e)
        {
            viewModel.InitFakeUser();
            this.Frame.NavigationFailed += OnNavigationFailed;
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

            Window.Current.Content = new SplitViewShell(this.Frame);
            this.Frame.Navigate(typeof(PastTripsMenuView));
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            //For now, don't let user go back to the log in page; need to finalize what this experience should be like when user keeps pushing back
            if (this.Frame.CurrentSourcePageType != typeof(PastTripsMenuView))
            {
                if (this.Frame != null && this.Frame.CanGoBack)
                {
                    e.Handled = true;
                    this.Frame.GoBack();
                }
            }
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void ShowUserWelcome()
        {
            if (viewModel.UserInfo?.FirstName != null && viewModel.UserInfo?.FirstName != string.Empty)
            {
                LoginButtons.Visibility = Visibility.Collapsed;
                SkipAuthBtn.Visibility = Visibility.Collapsed;
                AppLogo.Visibility = Visibility.Collapsed;
                WelcomeText.Text = "Welcome " + viewModel.UserInfo.FirstName + "!";
                WelcomeText.Visibility = Visibility.Visible;
                SetImageSource();
                ProfileImage.Visibility = Visibility.Visible;
                ContinueButton.Visibility = Visibility.Visible;
            }
            else  //if no user info to show, go directly to next page
            {
                Window.Current.Content = new SplitViewShell(this.Frame);
                this.Frame.Navigate(typeof(PastTripsMenuView));
        }
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.UserInfo == null)
            viewModel.InitFakeUser();

            Window.Current.Content = new SplitViewShell(this.Frame);
            this.Frame.Navigate(typeof(PastTripsMenuView));
        }

        private void SetImageSource()
        {
            ProfileImage.Source = new BitmapImage(new Uri(viewModel.UserInfo.ProfilePictureUri));
        }

    }
}
