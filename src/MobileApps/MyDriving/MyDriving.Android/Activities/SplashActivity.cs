
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using MyDriving.Utils;

namespace MyDriving.Droid.Activities
{
    [Activity(Label = "MyDriving", Theme="@style/SplashTheme", MainLauncher=true)]    
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Intent newIntent = null;
            if (Settings.Current.IsLoggedIn)
                newIntent = new Intent(this, typeof(MainActivity));
            else if (Settings.Current.FirstRun)
            {
#if XTC
                newIntent = new Intent(this, typeof(LoginActivity));

#else
                newIntent = new Intent(this, typeof(GettingStartedActivity));

#endif

#if !DEBUG
                Settings.Current.FirstRun = false;
#endif
            }
            else
                newIntent = new Intent(this, typeof(LoginActivity));


            newIntent.AddFlags(ActivityFlags.ClearTop);
            newIntent.AddFlags(ActivityFlags.SingleTop);
            StartActivity(newIntent);
            Finish();
        }
    }
}

