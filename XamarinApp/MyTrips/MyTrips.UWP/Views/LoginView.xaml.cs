using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Windows.UI.Popups;
using MyTrips.ViewModel;
using Windows.UI.Xaml.Media.Imaging;
using System.Net.Http;
using System.Net.Http.Headers;
using Windows.UI;
using Windows.UI.Core;


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
                    WelcomeText.Text = "Welcome. UserID = " + Utils.Settings.Current.UserId;
                    break;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
            WelcomeText.Text = "Welcome. UserID = " + Utils.Settings.Current.UserId;
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
            // Login the user and then load data from the mobile app.
            if (await AuthenticateWFacebookAsync())
            {
                LoginButtons.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                this.ShowSuccessfulLogin();
            }
            else
            {
                WelcomeText.Text = "Login failed";
            }
            WelcomeText.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private async void ShowSuccessfulLogin()
        {
            var dialog = new ContentDialog();
            dialog.Title = "Welcome!";
            dialog.Content = "UserID = " + user.UserId;
            dialog.PrimaryButtonText = "OK";
            var result = await dialog.ShowAsync();

            this.Frame.NavigationFailed += OnNavigationFailed;
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

            Window.Current.Content = new SplitViewShell(this.Frame);
            this.Frame.Navigate(typeof(PastTripsMenuView));
        }

        // Define a member variable for storing the signed-in user. 
        private MobileServiceUser user;

        // Define a method that performs the authentication process
        // using a Facebook sign-in. 
        private async System.Threading.Tasks.Task<bool> AuthenticateWFacebookAsync()
        {

            bool success = false;
            try
            {
                // Sign-in using Facebook authentication.
                user = await App.MobileService
                    .LoginAsync(MobileServiceAuthenticationProvider.Facebook);

                success = true;
            }
            catch (InvalidOperationException)
            {

            }
            return success;
        }

        //public async Task<string> GetFacebookUserDataAsync()
        //{
        //    HttpClient httpClient = CreateHttpClient();

        //    //string url = "https://graph.facebook.com/v2.5/me?fields=first_name&access_token=" + accessToken;
          

        //    string url = App.MobileAppsUrl + "/tables";
        //    var response = await httpClient.GetStringAsync(url);
        //    return response;
        //}

        //private HttpClient CreateHttpClient()
        //{
        //    var httpClient = new HttpClient();
        //    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //    return httpClient;
        //}

    }
}
