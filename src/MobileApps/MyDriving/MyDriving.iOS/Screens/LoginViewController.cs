// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using MyDriving.Utils;
using MyDriving.ViewModel;
using System;
using System.Threading.Tasks;
using UIKit;

namespace MyDriving.iOS
{
    partial class LoginViewController : UIViewController
    {
        bool didAnimate;
        LoginViewModel viewModel;

        public LoginViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            viewModel = new LoginViewModel();
            //Prepare buttons for fade in animation.
            btnFacebook.Alpha = 0;
            btnTwitter.Alpha = 0;
            btnMicrosoft.Alpha = 0;
            btnSkipAuth.Alpha = 0;

            btnSkipAuth.Layer.CornerRadius = 4;
            btnSkipAuth.Layer.MasksToBounds = true;

#if DEBUG || XTC
            btnSkipAuth.Hidden = false;
#else
            btnSkipAuth.Hidden = true;
#endif
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (didAnimate)
                return;

            didAnimate = true;
            btnFacebook.FadeIn(0.3, 0.3f);
            btnTwitter.FadeIn(0.3, 0.5f);
            btnMicrosoft.FadeIn(0.3, 0.7f);
            btnSkipAuth.FadeIn(0.3, 0.9f);
        }

        async partial void BtnFacebook_TouchUpInside(UIButton sender)
        {
            await LoginAsync(LoginAccount.Facebook);
        }

        async partial void BtnTwitter_TouchUpInside(UIButton sender)
        {
            await LoginAsync(LoginAccount.Twitter);
        }

        async partial void BtnMicrosoft_TouchUpInside(UIButton sender)
        {
            await LoginAsync(LoginAccount.Microsoft);
        }

        async Task LoginAsync(LoginAccount account)
        {
            switch (account)
            {
                case LoginAccount.Facebook:
                    await viewModel.ExecuteLoginFacebookCommandAsync();
                    break;
                case LoginAccount.Microsoft:
                    await viewModel.ExecuteLoginMicrosoftCommandAsync();
                    break;
                case LoginAccount.Twitter:
                    await viewModel.ExecuteLoginTwitterCommandAsync();
                    break;
            }

            if (viewModel.IsLoggedIn)
            {
                //When the first screen of the app is launched after user has logged in, initialize the processor that manages connection to OBD Device and to the IOT Hub
                Services.OBDDataProcessor.GetProcessor().Initialize(ViewModel.ViewModelBase.StoreManager);

                NavigateToTabs();
            }
        }

        partial void BtnSkipAuth_TouchUpInside(UIButton sender)
        {
            viewModel.InitFakeUser();
            NavigateToTabs();
        }

        void NavigateToTabs()
        {
            InvokeOnMainThread(() =>
            {
                var app = (AppDelegate) UIApplication.SharedApplication.Delegate;
                var viewController =
                    UIStoryboard.FromName("Main", null).InstantiateViewController("tabBarController") as
                        UITabBarController;
                viewController.SelectedIndex = 1;
                app.Window.RootViewController = viewController;
            });
        }
    }
}