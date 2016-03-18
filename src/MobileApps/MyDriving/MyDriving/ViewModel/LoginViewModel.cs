// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Threading.Tasks;
using System.Windows.Input;
using MyDriving.Utils;
using MyDriving.Helpers;
using MyDriving.Interfaces;
using MyDriving.DataObjects;
using Microsoft.WindowsAzure.MobileServices;
using MyDriving.AzureClient;
using System;

namespace MyDriving.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        readonly IAuthentication _authentication;
        private readonly IMobileServiceClient _client;

        bool _isLoggedIn;

        ICommand _loginFacebookCommand;

        ICommand _loginMicrosoftCommand;

        ICommand _loginTwitterCommand;

        public LoginViewModel()
        {
            _client = ServiceLocator.Instance.Resolve<IAzureClient>()?.Client;
            _authentication = ServiceLocator.Instance.Resolve<IAuthentication>();
        }

        public UserProfile UserProfile { get; set; }

        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set { SetProperty(ref _isLoggedIn, value); }
        }

        public ICommand LoginTwitterCommand =>
            _loginTwitterCommand ??
            (_loginTwitterCommand = new RelayCommand(async () => await ExecuteLoginTwitterCommandAsync()));

        public ICommand LoginMicrosoftCommand =>
            _loginMicrosoftCommand ??
            (_loginMicrosoftCommand = new RelayCommand(async () => await ExecuteLoginMicrosoftCommandAsync()));

        public ICommand LoginFacebookCommand =>
            _loginFacebookCommand ??
            (_loginFacebookCommand = new RelayCommand(async () => await ExecuteLoginFacebookCommandAsync()));

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
            if (_client == null || IsBusy)
                return;

            Settings.LoginAccount = LoginAccount.Twitter;
            var track = Logger.Instance.TrackTime("LoginTwitter");
            track?.Start();
            IsLoggedIn = await LoginAsync(MobileServiceAuthenticationProvider.Twitter);
            track?.Stop();
        }

        public async Task ExecuteLoginMicrosoftCommandAsync()
        {
            if (_client == null || IsBusy)
                return;

            Settings.LoginAccount = LoginAccount.Microsoft;
            var track = Logger.Instance.TrackTime("LoginMicrosoft");
            track?.Start();
            IsLoggedIn = await LoginAsync(MobileServiceAuthenticationProvider.MicrosoftAccount);
            track?.Stop();
        }

        public async Task ExecuteLoginFacebookCommandAsync()
        {
            if (_client == null || IsBusy)
                return;
            Settings.LoginAccount = LoginAccount.Facebook;
            var track = Logger.Instance.TrackTime("LoginFacebook");
            track?.Start();
            IsLoggedIn = await LoginAsync(MobileServiceAuthenticationProvider.Facebook);
            track?.Stop();
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
                _authentication.ClearCookies();
                user = await _authentication.LoginAsync(_client, provider);

                if (user != null)
                {
                    IsBusy = true;
                    UserProfile = await UserProfileHelper.GetUserProfileAsync(_client);
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