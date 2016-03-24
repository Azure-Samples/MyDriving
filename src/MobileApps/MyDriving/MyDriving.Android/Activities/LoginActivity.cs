// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
using MyDriving.ViewModel;
using MyDriving.Utils;
using Android.Support.V4.Content;
using Android.Graphics;

namespace MyDriving.Droid.Activities
{
    [Activity(Label = "Login", Theme = "@style/MyThemeDark",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class LoginActivity : BaseActivity
    {
        LoginViewModel viewModel;

        protected override int LayoutResource => Resource.Layout.activity_login;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if ((int) Build.VERSION.SdkInt >= 21)
            {
                Window.SetStatusBarColor(new Color(ContextCompat.GetColor(this, Resource.Color.primary_dark)));
                Window.DecorView.SystemUiVisibility = StatusBarVisibility.Visible;
            }

            viewModel = new LoginViewModel();
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
            var twitter = FindViewById<Button>(Resource.Id.button_twitter);
            var microsoft = FindViewById<Button>(Resource.Id.button_microsoft);
            var facebook = FindViewById<Button>(Resource.Id.button_facebook);

            twitter.Click += (sender, e) => Login(LoginAccount.Twitter);
            microsoft.Click += (sender, e) => Login(LoginAccount.Microsoft);
            facebook.Click += (sender, e) => Login(LoginAccount.Facebook);

            FindViewById<Button>(Resource.Id.button_skip).Click += (sender, e) =>
            {
                viewModel.InitFakeUser();
                var intent = new Intent(this, typeof(MainActivity));
                intent.AddFlags(ActivityFlags.ClearTop);
                StartActivity(intent);
                Finish();
            };

            #if XTC || DEBUG
            #else
            FindViewById<Button>(Resource.Id.button_skip).Visibility = ViewStates.Gone;
            #endif
            var typeface = Typeface.CreateFromAsset(Assets, "fonts/Corbert-Regular.otf");
            FindViewById<TextView>(Resource.Id.text_app_name).Typeface = typeface;
        }

        void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!viewModel.IsLoggedIn)
                return;

            //When the first screen of the app is launched after user has logged in, initialize the processor that manages connection to OBD Device and to the IOT Hub
            MyDriving.Services.OBDDataProcessor.GetProcessor().Initialize(ViewModel.ViewModelBase.StoreManager);

            var intent = new Intent(this, typeof (MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            StartActivity(intent);
            Finish();
        }


        void Login(LoginAccount account)
        {
            switch (account)
            {
                case LoginAccount.Facebook:
                    viewModel.LoginFacebookCommand.Execute(null);
                    break;
                case LoginAccount.Microsoft:
                    viewModel.LoginMicrosoftCommand.Execute(null);
                    break;
                case LoginAccount.Twitter:
                    viewModel.LoginTwitterCommand.Execute(null);
                    break;
            }
        }
    }
}