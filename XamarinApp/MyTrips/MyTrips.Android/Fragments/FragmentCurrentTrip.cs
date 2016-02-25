using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using MyTrips.Droid.Services;
using MyTrips.ViewModel;
using MvvmHelpers;
using MyTrips.DataObjects;


namespace MyTrips.Droid.Fragments
{
    public class FragmentCurrentTrip : Fragment
    {
        TextView latText;
        TextView longText;
        TextView altText;

        public static FragmentCurrentTrip NewInstance()
        {
            var frag1 = new FragmentCurrentTrip { Arguments = new Bundle() };
            return frag1;
        }

        CurrentTripViewModel viewModel;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.fragment_current_trip, null);

            GeolocationHelper.Current.LocationServiceConnected += (sender, e) =>
                {
                    viewModel = GeolocationHelper.Current.LocationService.ViewModel;
                    var list = viewModel.CurrentTrip.Trail as ObservableRangeCollection<Trail>;
                    list.CollectionChanged += TrailUpdated;
                };
            latText = view.FindViewById<TextView>(Resource.Id.lat);
            longText = view.FindViewById<TextView>(Resource.Id.longx);
            altText = view.FindViewById<TextView>(Resource.Id.alt);

            return view;
        }

        void TrailUpdated (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Activity.RunOnUiThread (() => {
                var item = viewModel.CurrentTrip.Trail[viewModel.CurrentTrip.Trail.Count - 1];
                latText.Text = $"Latitude: {item.Latitude}";
                longText.Text = $"Longitude: {item.Longitude}";
                altText.Text = $"Altitude: {item.TimeStamp}";
            });
        }

        public override void OnStart()
        {
            base.OnStart();
            GeolocationHelper.StartLocationService();
        }

        public override void OnStop()
        {
            base.OnStop();
            GeolocationHelper.Current.LocationService.StopLocationUpdates();
            GeolocationHelper.StopLocationService();
        }
    }
}