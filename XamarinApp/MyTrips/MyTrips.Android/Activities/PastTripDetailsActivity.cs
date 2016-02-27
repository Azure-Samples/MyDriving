using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Content.PM;
using Android.Graphics;
using Android.Support.V4.Content;
using Android.Gms.Maps;
using MyTrips.ViewModel;
using Android.Gms.Maps.Model;

namespace MyTrips.Droid.Activities
{
    [Activity(Label = "Details", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]            
    public class PastTripDetailsActivity : BaseActivity, IOnMapReadyCallback  
    {
        protected override int LayoutResource
        {
            get
            {
                return Resource.Layout.activity_past_trip_details;
            }
        }

        GoogleMap map;
        TripDetailsViewModel viewModel;
        SupportMapFragment mapFrag;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            if ((int)Build.VERSION.SdkInt >= 21)
            {
                Window.SetStatusBarColor(new Color(ContextCompat.GetColor(this, Resource.Color.primary_dark)));
                Window.DecorView.SystemUiVisibility = StatusBarVisibility.Visible;
            }

            viewModel = new TripDetailsViewModel();
            viewModel.TripId = Intent.GetStringExtra("Id");
            mapFrag = (SupportMapFragment) SupportFragmentManager.FindFragmentById(Resource.Id.map);
            mapFrag.GetMapAsync(this);
        }

        public async void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;

            await viewModel.ExecuteLoadTripCommandAsync();

            SupportActionBar.Title = viewModel.CurrentTrip.TripId;
            SetupMap();

        }

        MarkerOptions car;
        void SetupMap()
        {

            if (mapFrag.View.Width == 0)
            {
                mapFrag.View.PostDelayed (() => { SetupMap();}, 500);
                return;
            }
            var start = viewModel.CurrentTrip.Trail[0];

            car = new MarkerOptions();
            car.SetPosition(new LatLng(start.Latitude, start.Longitude));
            car.SetTitle("The Car");
            car.SetIcon(BitmapDescriptorFactory.DefaultMarker (BitmapDescriptorFactory.HueCyan));
            map.AddMarker(car);

            var points = viewModel.CurrentTrip.Trail.Select(s => new LatLng(s.Latitude, s.Longitude)).ToArray();
            var rectOptions = new PolylineOptions();
            rectOptions.Add(points);
            rectOptions.InvokeColor(ContextCompat.GetColor(this, Resource.Color.accent));
            map.AddPolyline(rectOptions);

         

            var boundsPoints = new LatLngBounds.Builder ();
            foreach(var point in points)
                boundsPoints.Include (point);

            var bounds = boundsPoints.Build ();
            map.MoveCamera (CameraUpdateFactory.NewLatLngBounds (bounds, 56));

        }

        public override bool OnOptionsItemSelected (IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
                Finish ();

            return base.OnOptionsItemSelected (item);
        }
    }
}

