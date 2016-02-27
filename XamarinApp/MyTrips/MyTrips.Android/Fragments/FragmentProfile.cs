using Android.OS;
using Android.Support.V4.App;
using Android.Views;


namespace MyTrips.Droid.Fragments
{
    public class FragmentProfile : Fragment
    {
        public static FragmentProfile NewInstance() => new FragmentProfile { Arguments = new Bundle() };

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            return inflater.Inflate(Resource.Layout.fragment_profile, null);
        }
    }
}