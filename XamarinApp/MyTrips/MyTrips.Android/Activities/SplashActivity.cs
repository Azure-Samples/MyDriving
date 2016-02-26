
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using MyTrips.Utils;

namespace MyTrips.Droid
{
    [Activity(Label = "My Trips", Theme="@style/SplashTheme", MainLauncher=true)]    
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if(Settings.Current.IsLoggedIn)
                StartActivity(new Intent(this, typeof(MainActivity)));
            else
                StartActivity(new Intent(this, typeof(LoginActivity)));
            Finish();
        }
    }
}

