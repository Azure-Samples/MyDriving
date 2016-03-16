using System.Threading.Tasks;
using System.Windows.Input;
using MyTrips.Utils;
using MyTrips.Helpers;
using MyTrips.Interfaces;
using MyTrips.DataObjects;
using Microsoft.WindowsAzure.MobileServices;
using MyTrips.DataStore.Abstractions;
using MyTrips.AzureClient;
using System;

namespace MyTrips.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private IMobileServiceClient client;
        IAuthentication authentication;
        public LoginViewModel()
        {
            client = ServiceLocator.Instance.Resolve<IAzureClient>()?.Client;
            authentication = ServiceLocator.Instance.Resolve<IAuthentication>();
        }

        public UserProfile UserProfile { get; set; }

        bool isLoggedIn;
        public bool IsLoggedIn
        {
            get { return isLoggedIn; }
            set { SetProperty(ref isLoggedIn, value); }
        }

        public void InitFakeUser()
        {
            Init(true);
            Settings.UserFirstName = "Scott";
            Settings.UserLastName = "Gu";
            Settings.UserProfileUrl = "http://refractored.com/images/Scott.png";
        }

        ICommand  loginTwitterCommand;
        public ICommand LoginTwitterCommand =>
            loginTwitterCommand ?? (loginTwitterCommand = new RelayCommand(async () => await ExecuteLoginTwitterCommandAsync())); 

        public async Task ExecuteLoginTwitterCommandAsync()
        {
            if(client == null || IsBusy)
                return;
            
            Settings.LoginAccount = LoginAccount.Twitter;
            var track = Logger.Instance.TrackTime("LoginTwitter");
            track.Start();
            IsLoggedIn = await LoginAsync(MobileServiceAuthenticationProvider.Twitter);
            track.Stop();
        }

        ICommand  loginMicrosoftCommand;
        public ICommand LoginMicrosoftCommand =>
            loginMicrosoftCommand ?? (loginMicrosoftCommand = new RelayCommand(async () => await ExecuteLoginMicrosoftCommandAsync())); 

        public async Task ExecuteLoginMicrosoftCommandAsync()
        {
            if(client == null || IsBusy)
                return;
            
            Settings.LoginAccount = LoginAccount.Microsoft;
            var track = Logger.Instance.TrackTime("LoginMicrosoft");
            track.Start();
            IsLoggedIn = await LoginAsync(MobileServiceAuthenticationProvider.MicrosoftAccount);
            track.Stop();
        }

        ICommand  loginFacebookCommand;
        public ICommand LoginFacebookCommand =>
            loginFacebookCommand ?? (loginFacebookCommand = new RelayCommand(async () => await ExecuteLoginFacebookCommandAsync())); 

        public async Task ExecuteLoginFacebookCommandAsync()
        {
            if(client == null || IsBusy)
                return;
            Settings.LoginAccount = LoginAccount.Facebook;
            var track = Logger.Instance.TrackTime("LoginFacebook");
            track.Start();
            IsLoggedIn = await LoginAsync(MobileServiceAuthenticationProvider.Facebook);
            track.Stop();
        }

        async Task<bool> LoginAsync(MobileServiceAuthenticationProvider provider)
        {
            if (!Plugin.Connectivity.CrossConnectivity.Current.IsConnected)
            {                
                Acr.UserDialogs.UserDialogs.Instance.Alert("Ensure you have internet connection to login.",
                    "No Connection", "OK");
                
                return false;
            }

            MobileServiceUser user = null;

            try
            {
                authentication.ClearCookies();
                user = await authentication.LoginAsync(client, provider);

                if (user != null)
                {
                    IsBusy = true;
                    UserProfile = await UserProfileHelper.GetUserProfileAsync(client);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Report(ex);
            }
            finally
            {
                IsBusy = false;
            }

            if (user == null || UserProfile == null)
            {
                Settings.LoginAccount = LoginAccount.None;
                Settings.UserFirstName = string.Empty;
                Settings.AuthToken = string.Empty;
                Settings.UserLastName = string.Empty;
                Acr.UserDialogs.UserDialogs.Instance.Alert("Unable to login or create account.", "Login error", "OK");
                return false;
            }
            else
            {
                Init();
            }

            return true;
        }
    }
}
