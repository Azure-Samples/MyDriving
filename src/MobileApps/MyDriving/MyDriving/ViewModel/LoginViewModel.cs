// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Threading.Tasks;
using System.Windows.Input;
using MyDriving.Utils;
using MyDriving.Helpers;
using MyDriving.DataObjects;
using Microsoft.WindowsAzure.MobileServices;
using MyDriving.AzureClient;
using System;
using MyDriving.Utils.Interfaces;

namespace MyDriving.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        readonly IAuthentication authentication;
        private readonly IMobileServiceClient client;

        bool isLoggedIn;

        ICommand loginFacebookCommand;

        ICommand loginMicrosoftCommand;

        ICommand loginTwitterCommand;

        public LoginViewModel()
        {
            client = ServiceLocator.Instance.Resolve<IAzureClient>()?.Client;
            authentication = ServiceLocator.Instance.Resolve<IAuthentication>();
        }

        public UserProfile UserProfile { get; set; }

        public bool IsLoggedIn
        {
            get { return isLoggedIn; }
            set { SetProperty(ref isLoggedIn, value); }
        }

        public ICommand LoginTwitterCommand =>
            loginTwitterCommand ??
            (loginTwitterCommand = new RelayCommand(async () => await ExecuteLoginTwitterCommandAsync()));

        public ICommand LoginMicrosoftCommand =>
            loginMicrosoftCommand ??
            (loginMicrosoftCommand = new RelayCommand(async () => await ExecuteLoginMicrosoftCommandAsync()));

        public ICommand LoginFacebookCommand =>
            loginFacebookCommand ??
            (loginFacebookCommand = new RelayCommand(async () => await ExecuteLoginFacebookCommandAsync()));

        public void InitFakeUser()
        {
            Init(true);
            Settings.UserFirstName = "Scott";
            Settings.UserLastName = "Gu";
            Settings.UserUID = "1";
            Settings.UserProfileUrl = "http://refractored.com/images/Scott.png";
        }

        public async Task ExecuteLoginTwitterCommandAsync()
        {
            if (client == null || IsBusy)
                return;

            Settings.LoginAccount = LoginAccount.Twitter;
            var track = Logger.Instance.TrackTime("LoginTwitter");
            track?.Start();
            IsLoggedIn = await LoginAsync(MobileServiceAuthenticationProvider.Twitter);
            track?.Stop();
            Logger.Instance.Track("LoginTwitter");
        }

        public async Task ExecuteLoginMicrosoftCommandAsync()
        {
            if (client == null || IsBusy)
                return;

            Settings.LoginAccount = LoginAccount.Microsoft;
            var track = Logger.Instance.TrackTime("LoginMicrosoft");
            track?.Start();
            IsLoggedIn = await LoginAsync(MobileServiceAuthenticationProvider.MicrosoftAccount);
            track?.Stop();
            Logger.Instance.Track("LoginMicrosoft");
        }

        public async Task ExecuteLoginFacebookCommandAsync()
        {
            if (client == null || IsBusy)
                return;
            Settings.LoginAccount = LoginAccount.Facebook;
            var track = Logger.Instance.TrackTime("LoginFacebook");
            track?.Start();
            IsLoggedIn = await LoginAsync(MobileServiceAuthenticationProvider.Facebook);
            track?.Stop();
            Logger.Instance.Track("LoginFacebook");
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

                Logger.Instance.Track("LoginError");
                Acr.UserDialogs.UserDialogs.Instance.Alert("Unable to login or create account.", "Login error", "OK");
                return false;
            }
            else
            {
                Init();

                Logger.Instance.Track("LoginSuccess");
            }

            return true;
        }
    }
}