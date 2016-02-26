using Android.OS;
using Android.Support.V4.App;
using Android.Views;


namespace MyTrips.Droid.Fragments
{
    public class FragmentSettings : Fragment
    {
     
        public static FragmentSettings NewInstance() => new FragmentSettings { Arguments = new Bundle() };


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            return inflater.Inflate(Resource.Layout.fragment_settings, null);
        }
    }
}