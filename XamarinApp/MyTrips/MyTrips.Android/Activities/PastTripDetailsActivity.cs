using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
using Android.Graphics;
using Android.Support.V4.Content;
using Android.Gms.Maps;
using MyTrips.ViewModel;
using Android.Gms.Maps.Model;
using MyTrips.Droid.Controls;
using Android.Graphics.Drawables;
using System;

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
        TextView ratingText;
        RatingCircle ratingCircle;
        int rating;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            if ((int)Build.VERSION.SdkInt >= 21)
            {
                Window.SetStatusBarColor(new Color(ContextCompat.GetColor(this, Resource.Color.primary_dark)));
                Window.DecorView.SystemUiVisibility = StatusBarVisibility.Visible;
            }

            var view = FindViewById(Resource.Id.full_rating);
            Android.Support.V4.View.ViewCompat.SetTransitionName(view,  "rating");

            viewModel = new TripDetailsViewModel();
            viewModel.TripId = Intent.GetStringExtra("Id");
            rating = Intent.GetIntExtra("Rating", 0);
            ratingText = FindViewById<TextView>(Resource.Id.text_rating);
            ratingCircle = FindViewById<RatingCircle>(Resource.Id.rating_circle);
            seekBar = FindViewById<SeekBar>(Resource.Id.trip_progress);
            seekBar.Enabled = false;
            ratingCircle.PlayAnimation = false;
            ratingText.Text = rating.ToString();
            ratingCircle.Rating = rating;
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

        Marker carMarker;
        SeekBar seekBar;
        void SetupMap()
        {

            if (mapFrag.View.Width == 0)
            {
                mapFrag.View.PostDelayed (() => { SetupMap();}, 500);
                return;
            }
            var start = viewModel.CurrentTrip.Trail[0];
            var end = viewModel.CurrentTrip.Trail[viewModel.CurrentTrip.Trail.Count - 1];
            seekBar.Max = viewModel.CurrentTrip.Trail.Count - 1;
            seekBar.ProgressChanged += SeekBar_ProgressChanged;

            var logicalDensity = Resources.DisplayMetrics.Density;
            var thicknessCar = (int)Math.Ceiling(24 * logicalDensity + .5f);
            var thicknessPoints = (int)Math.Ceiling(20 * logicalDensity + .5f);

            var b = ContextCompat.GetDrawable(this, Resource.Drawable.ic_car_blue) as BitmapDrawable;
            var finalIcon = Bitmap.CreateScaledBitmap(b.Bitmap, thicknessCar, thicknessCar, false);

            var car = new MarkerOptions();
            car.SetPosition(new LatLng(start.Latitude, start.Longitude));
            car.SetIcon(BitmapDescriptorFactory.FromBitmap(finalIcon));
            car.Anchor(.5f, .5f);

            b = ContextCompat.GetDrawable(this, Resource.Drawable.ic_start_point) as BitmapDrawable;
            finalIcon = Bitmap.CreateScaledBitmap(b.Bitmap, thicknessPoints, thicknessPoints, false);

            var startMarker = new MarkerOptions();
            startMarker.SetPosition(new LatLng(start.Latitude, start.Longitude));
            startMarker.SetIcon(BitmapDescriptorFactory.FromBitmap(finalIcon));
            startMarker.Anchor(.5f, .5f);

            b = ContextCompat.GetDrawable(this, Resource.Drawable.ic_end_point) as BitmapDrawable;
            finalIcon = Bitmap.CreateScaledBitmap(b.Bitmap, thicknessPoints, thicknessPoints, false);

            var endMarker = new MarkerOptions();
            endMarker.SetPosition(new LatLng(end.Latitude, end.Longitude));
            endMarker.SetIcon(BitmapDescriptorFactory.FromBitmap(finalIcon));
            endMarker.Anchor(.5f, .5f);



            var points = viewModel.CurrentTrip.Trail.Select(s => new LatLng(s.Latitude, s.Longitude)).ToArray();
            var rectOptions = new PolylineOptions();
            rectOptions.Add(points);
            rectOptions.InvokeColor(ContextCompat.GetColor(this, Resource.Color.accent));
            map.AddPolyline(rectOptions);


            map.AddMarker(startMarker);
            map.AddMarker(endMarker);

            carMarker = map.AddMarker(car);

            var boundsPoints = new LatLngBounds.Builder ();
            foreach(var point in points)
                boundsPoints.Include (point);

            var bounds = boundsPoints.Build ();
            map.MoveCamera (CameraUpdateFactory.NewLatLngBounds (bounds, 128));


            seekBar.Enabled = true;

        }

        void SeekBar_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            if (carMarker == null)
                return;
            var location = viewModel.CurrentTrip.Trail[e.Progress];

            RunOnUiThread(() =>
            {
                carMarker.Position = new LatLng(location.Latitude, location.Longitude);
            });
        }

        public override bool OnOptionsItemSelected (IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
                Finish ();

            return base.OnOptionsItemSelected (item);
        }
    }
}

