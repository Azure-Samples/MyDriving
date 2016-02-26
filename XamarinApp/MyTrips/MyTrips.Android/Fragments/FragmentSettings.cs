using Android.OS;
using Android.Support.V7.Preferences;


namespace MyTrips.Droid.Fragments
{
    public class FragmentSettings : PreferenceFragmentCompat
    {
     
        public static FragmentSettings NewInstance() => new FragmentSettings { Arguments = new Bundle() };

        public override void OnCreatePreferences(Bundle p0, string p1)
        {
            AddPreferencesFromResource(Resource.Xml.preferences);
        }
    }
}