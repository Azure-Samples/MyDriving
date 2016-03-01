
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
using MyTrips.ViewModel;
using MyTrips.Utils;
using Android.Support.V4.Content;
using Android.Graphics;

namespace MyTrips.Droid
{
    [Activity(Label = "Login", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]            
    public class LoginActivity : BaseActivity
    {
        protected override int LayoutResource
        {
            get
            {
                return Resource.Layout.activity_login;
            }
        }

        LoginViewModel viewModel;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if ((int)Build.VERSION.SdkInt >= 21)
            {
                Window.SetStatusBarColor(new Color(ContextCompat.GetColor(this, Resource.Color.primary_dark)));
                Window.DecorView.SystemUiVisibility = StatusBarVisibility.Visible;
            }

            viewModel = new LoginViewModel();
            var twitter = FindViewById<Button>(Resource.Id.button_twitter);
            var microsoft = FindViewById<Button>(Resource.Id.button_microsoft);
            var facebook = FindViewById<Button>(Resource.Id.button_facebook);

            twitter.Click += (sender, e) => Login(LoginAccount.Twitter);
            microsoft.Click += (sender, e) => Login(LoginAccount.Microsoft);
            facebook.Click += (sender, e) => Login(LoginAccount.Facebook);

            FindViewById<Button>(Resource.Id.button_skip).Click += (sender, e) => 
            {
                var intent = new Intent(this, typeof(MainActivity));
                intent.AddFlags(ActivityFlags.ClearTop);
                StartActivity(intent);
                Finish();
            };

            SupportActionBar.SetDisplayHomeAsUpEnabled(false);
            SupportActionBar.SetDisplayShowHomeEnabled(false);
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (!viewModel.IsLoggedIn)
                return;

            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            StartActivity(intent);
            Finish();
        }

        void Login(LoginAccount account)
        {
            #if DEBUG
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            StartActivity(intent);
            Finish();
            #endif

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

