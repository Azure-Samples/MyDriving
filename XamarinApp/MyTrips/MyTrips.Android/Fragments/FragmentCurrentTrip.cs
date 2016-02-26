using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using MyTrips.Droid.Services;
using MyTrips.ViewModel;
using MvvmHelpers;
using MyTrips.DataObjects;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using System.Linq;


namespace MyTrips.Droid.Fragments
{
    public class FragmentCurrentTrip : Fragment, IOnMapReadyCallback  
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
        GoogleMap map;
        MapView mapView;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.fragment_current_trip, null);

            mapView = view.FindViewById<MapView>(Resource.Id.map);
            mapView.OnCreate(savedInstanceState);
            mapView.GetMapAsync(this);

            GeolocationHelper.Current.LocationServiceConnected += (sender, e) =>
                {
                    viewModel = GeolocationHelper.Current.LocationService.ViewModel;
                    var list = viewModel.CurrentTrip.Trail as ObservableRangeCollection<Trail>;
                    list.CollectionChanged += TrailUpdated;

                    if(car == null)
                        SetupMap();
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

                if(car != null)
                    UpdateMap(item);
                else
                    SetupMap();
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

        public void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;
            if(viewModel != null)
                SetupMap();
        }

        MarkerOptions car;
        void SetupMap()
        {
            if (map == null)
                return;
            
            if (viewModel?.CurrentTrip?.Trail.Count == 0)
                return;
            
            var start = viewModel.CurrentTrip.Trail[0];
            car = new MarkerOptions();
            car.SetPosition(new LatLng(start.Latitude, start.Longitude));
            car.SetTitle("The Car");
            car.SetIcon(BitmapDescriptorFactory.DefaultMarker (BitmapDescriptorFactory.HueCyan));
            map.AddMarker(car);

            var points = viewModel.CurrentTrip.Trail.Select(s => new LatLng(s.Latitude, s.Longitude)).ToArray();
            var rectOptions = new PolylineOptions();
            rectOptions.Add(points);
            map.AddPolyline(rectOptions);
            UpdateCamera();
        }

        void UpdateMap(Trail trail)
        {
            if(map == null)
                return;
            var latlng = new LatLng(trail.Latitude, trail.Longitude);
            car.SetPosition(latlng);

            map.Clear();
            var points = viewModel.CurrentTrip.Trail.Select(s => new LatLng(s.Latitude, s.Longitude)).ToArray();
            var rectOptions = new PolylineOptions();
            rectOptions.Add(points);
            map.AddPolyline(rectOptions);
            UpdateCamera();
        }

        void UpdateCamera()
        {
            var boundsPoints = new LatLngBounds.Builder ();
            var points = viewModel.CurrentTrip.Trail.Select(s => new LatLng(s.Latitude, s.Longitude)).ToArray();
            foreach(var point in points)
                boundsPoints.Include (point);

            var bounds = boundsPoints.Build ();
            map.MoveCamera (CameraUpdateFactory.NewLatLngBounds (bounds, 24));
        }

        public override void OnResume()
        {
            base.OnResume();
            mapView.OnResume();
        }

        public override void OnPause()
        {
            base.OnPause();
            mapView.OnPause();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            mapView.OnDestroy();
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            mapView.OnSaveInstanceState(outState);
        }

        public override void OnLowMemory()
        {
            base.OnLowMemory();
            mapView.OnLowMemory();
        }
    }
}