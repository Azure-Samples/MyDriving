﻿using System.Threading.Tasks;
using System.Windows.Input;
using MyTrips.Utils;
using MyTrips.Helpers;
using MyTrips.Interfaces;
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

        UserProfile userInfo;
        public UserProfile UserInfo
        {
            get { return userInfo; }
            set { SetProperty(ref userInfo, value); }
        }



        bool isLoggedIn;
        public bool IsLoggedIn
        {
            get { return isLoggedIn; }
            set { SetProperty(ref isLoggedIn, value); }
        }

        public void InitFakeUser()
        {
            Settings.UserFirstName = "Scott";
            Settings.UserLastName = "Gu";
            Settings.UserProfileUrl = "http://refractored.com/images/Scott.png";
            Settings.UserPictureSourceKind = UserPictureSourceKind.Url;
        }

        ICommand  loginTwitterCommand;
        public ICommand LoginTwitterCommand =>
            loginTwitterCommand ?? (loginTwitterCommand = new RelayCommand(async () => await ExecuteLoginTwitterCommandAsync())); 

        async Task ExecuteLoginTwitterCommandAsync()
        {
            if(client == null)
                return;

            var track = Logger.Instance.TrackTime("LoginTwitter");
            track.Start();

            Settings.LoginAccount = LoginAccount.Twitter;
            MobileServiceUser user = null;
            try
            {
                user = await authentication.LoginAsync(client, MobileServiceAuthenticationProvider.Twitter);
                if(user != null)
                    UserInfo = await UserProfileHelper.GetUserProfileAsync(client);
            }
            catch (Exception ex)
            {
                Logger.Instance.Report(ex);
            }
            track.Stop();

            if(user == null)
            {
                Settings.LoginAccount = LoginAccount.None;
                return;
            }

            IsLoggedIn = true;
        }


        ICommand  loginMicrosoftCommand;
        public ICommand LoginMicrosoftCommand =>
            loginMicrosoftCommand ?? (loginMicrosoftCommand = new RelayCommand(async () => await ExecuteLoginMicrosoftCommandAsync())); 

        async Task ExecuteLoginMicrosoftCommandAsync()
        {
            if(client == null)
                return;

            var track = Logger.Instance.TrackTime("LoginMicrosoft");
            track.Start();


            Settings.LoginAccount = LoginAccount.Microsoft;
            MobileServiceUser user = null;
            try
            {
                user = user = await authentication.LoginAsync(client, MobileServiceAuthenticationProvider.MicrosoftAccount);
                if (user != null)
                    UserInfo = await UserProfileHelper.GetUserProfileAsync(client);
            }
            catch (Exception ex)
            {
                Logger.Instance.Report(ex);
            }
            track.Stop();

            if(user == null)
            {
                Settings.LoginAccount = LoginAccount.None;
                return;
            }

            IsLoggedIn = true;
            
        }

        ICommand  loginFacebookCommand;
        public ICommand LoginFacebookCommand =>
            loginFacebookCommand ?? (loginFacebookCommand = new RelayCommand(async () => await ExecuteLoginFacebookCommandAsync())); 

        async Task ExecuteLoginFacebookCommandAsync()
        {
            if(client == null)
                return;
            var track = Logger.Instance.TrackTime("LoginFacebook");
            track.Start();
            Settings.LoginAccount = LoginAccount.Facebook;
            MobileServiceUser user = null;
            try
            {
                user = await authentication.LoginAsync(client, MobileServiceAuthenticationProvider.Facebook);
                if(user != null)
                    UserInfo = await UserProfileHelper.GetUserProfileAsync(client);
            }
            catch (Exception ex)
            {
                Logger.Instance.Report(ex);
            }
            track.Stop();

            if(user == null)
            {
                Settings.LoginAccount = LoginAccount.None;
                return;
            }


            IsLoggedIn = true;
        }
    }
}
