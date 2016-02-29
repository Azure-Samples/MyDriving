using Android.OS;
using Android.Support.V7.Preferences;
using Android.Content;
using Android.Views;
using Android.App;
using MyTrips.ViewModel;

namespace MyTrips.Droid.Fragments
{
    public class FragmentSettings : PreferenceFragmentCompat
    {

        SettingsViewModel viewModel;
        public static FragmentSettings NewInstance() => new FragmentSettings { Arguments = new Bundle() };

        public override void OnCreatePreferences(Bundle p0, string p1)
        {
            AddPreferencesFromResource(Resource.Xml.preferences);
            viewModel = new SettingsViewModel();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var logout = FindPreference("logout");
            logout.PreferenceClick += async (sender, e) =>
            {
                if (!(await viewModel.ExecuteLogoutCommandAsync()))
                    return;
                //Logged out!
                var intent = new Intent(Activity, typeof(LoginActivity));
                intent.AddFlags(ActivityFlags.ClearTop);
                Activity.StartActivity(intent);
                Activity.Finish();
            };
        }
    }
}