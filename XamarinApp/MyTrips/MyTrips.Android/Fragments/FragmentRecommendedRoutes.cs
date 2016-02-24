using Android.OS;
using Android.Support.V4.App;
using Android.Views;


namespace MyTrips.Droid.Fragments
{
    public class FragmentRecommendedRoutes : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public static FragmentRecommendedRoutes NewInstance()
        {
            var frag1 = new FragmentRecommendedRoutes { Arguments = new Bundle() };
            return frag1;
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var ignored = base.OnCreateView(inflater, container, savedInstanceState);
            return inflater.Inflate(Resource.Layout.fragment_routes, null);
        }
    }
}