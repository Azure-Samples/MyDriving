using Android.OS;
using Android.Support.V7.Preferences;
using Android.Content;
using Android.Views;
using Android.App;

namespace MyTrips.Droid.Fragments
{
    public class FragmentSettings : PreferenceFragmentCompat
    {
     
        public static FragmentSettings NewInstance() => new FragmentSettings { Arguments = new Bundle() };

        public override void OnCreatePreferences(Bundle p0, string p1)
        {
            AddPreferencesFromResource(Resource.Xml.preferences);
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var logout = FindPreference("logout");
            logout.PreferenceClick += (sender, e) =>
            {
                var builder = new AlertDialog.Builder(Activity);
                builder
                    .SetTitle("Logout")
                    .SetMessage("Are you sure you want to logout?")
                    .SetPositiveButton(Android.Resource.String.Ok, delegate
                {
                    var intent = new Intent(Activity, typeof(LoginActivity));
                    intent.AddFlags(ActivityFlags.ClearTop);
                    Activity.StartActivity(intent);
                    Activity.Finish();

                }).SetNegativeButton(Android.Resource.String.Cancel, delegate
                {

                });

                var alert = builder.Create();
                alert.Show();
            };
        }
    }
}