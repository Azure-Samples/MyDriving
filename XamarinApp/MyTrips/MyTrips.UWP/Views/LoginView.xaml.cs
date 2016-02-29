using Microsoft.WindowsAzure.MobileServices;
using MyTrips.Helpers;
using MyTrips.Utils;
using MyTrips.ViewModel;
using System;
using System.Threading.Tasks;
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
                case nameof(viewModel.UserInfo):
                    WelcomeText.Text = "Welcome " + viewModel.UserInfo.FirstName + "!";
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

        private async void FaceBookButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            await Login(LoginAccount.Facebook);
        }

        private async void TwitterButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            await Login(LoginAccount.Twitter);
        }

        private async void MicrosoftButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            await Login(LoginAccount.Microsoft);
        }

        private async Task Login(LoginAccount provider)
        {
            // Login the user and then load data from the mobile app.
            if (await AuthenticateAsync(provider))
            {
                viewModel.UserInfo = await UserProfileHelper.GetUserProfileAsync(App.MobileService);
                this.ShowSuccessfulLogin();
            }
            else
            {
                WelcomeText.Text = "Login failed";
            }
            WelcomeText.Visibility = Visibility.Visible;
        }

        private void ShowSuccessfulLogin()
        {
            LoginButtons.Visibility = Visibility.Collapsed;
            WelcomeText.Visibility = Visibility.Visible;
            ContinueButton.Visibility = Visibility.Visible;
            SkipAuthBtn.Visibility = Visibility.Collapsed;
            ProfileImage.Source = new BitmapImage(new Uri(viewModel.UserInfo.ProfilePictureUri));
            ProfileImage.Visibility = Visibility.Visible;
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(PastTripsMenuView));
        }

        // Define a member variable for storing the signed-in user. 
        private MobileServiceUser user;

        //Temp code to use for login until we're properly connected to mobile server 
        private async Task<bool> AuthenticateAsync(LoginAccount provider)
        {

            bool success = true;
            try
            {
                switch (provider)
                {
                    case LoginAccount.Facebook:
                        user = await App.MobileService
                   .LoginAsync(MobileServiceAuthenticationProvider.Facebook);
                        break;
                    case LoginAccount.Twitter:
                        user = await App.MobileService
                   .LoginAsync(MobileServiceAuthenticationProvider.Twitter);
                        break;
                    case LoginAccount.Microsoft:
                        user = await App.MobileService
                   .LoginAsync(MobileServiceAuthenticationProvider.MicrosoftAccount);
                        break;
                    default:
                        success = false;
                        break;
                }
            }
            catch (InvalidOperationException)
            {
                success = false;
            }
            return success;
        }

    }
}
