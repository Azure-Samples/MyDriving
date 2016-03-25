// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
using Android.Graphics;
using Android.Support.V4.Content;
using Android.Gms.Maps;
using MyDriving.ViewModel;
using Android.Gms.Maps.Model;
using Android.Graphics.Drawables;
using System;
using MyDriving.DataObjects;

namespace MyDriving.Droid.Activities
{
    [Activity(Label = "Details", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class PastTripDetailsActivity : BaseActivity, IOnMapReadyCallback
    {
        Marker carMarker;
        TextView distance, distanceUnits, time, speed, speedUnits, consumption, consumptionUnits;
        string id;

        GoogleMap map;
        SupportMapFragment mapFrag;
        SeekBar seekBar;
        TextView startTime, endTime;
        PastTripsDetailViewModel viewModel;

        protected override int LayoutResource => Resource.Layout.activity_past_trip_details;


        public async void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;

            var success = await viewModel.ExecuteLoadTripCommandAsync(id);
            if (!success)
            {
                Finish();
                return;
            }

            startTime.Text = viewModel.Trip.StartTimeDisplay;
            endTime.Text = viewModel.Trip.EndTimeDisplay;
            SupportActionBar.Title = viewModel.Title;
            SetupMap();
            UpdateStats();
        }
        public static Trip Trip { get; set; }
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            if ((int) Build.VERSION.SdkInt >= 21)
            {
                Window.SetStatusBarColor(new Color(ContextCompat.GetColor(this, Resource.Color.primary_dark)));
                Window.DecorView.SystemUiVisibility = StatusBarVisibility.Visible;
            }


            viewModel = new PastTripsDetailViewModel {Title = id = Intent.GetStringExtra("Id")};

            viewModel.Trip = Trip;
            seekBar = FindViewById<SeekBar>(Resource.Id.trip_progress);
            seekBar.Enabled = false;

            startTime = FindViewById<TextView>(Resource.Id.text_start_time);
            endTime = FindViewById<TextView>(Resource.Id.text_end_time);
            startTime.Text = endTime.Text = string.Empty;

            time = FindViewById<TextView>(Resource.Id.text_time);
            distance = FindViewById<TextView>(Resource.Id.text_distance);
            distanceUnits = FindViewById<TextView>(Resource.Id.text_distance_units);
            consumption = FindViewById<TextView>(Resource.Id.text_consumption);
            consumptionUnits = FindViewById<TextView>(Resource.Id.text_consumption_units);
            speed = FindViewById<TextView>(Resource.Id.text_speed);
            speedUnits = FindViewById<TextView>(Resource.Id.text_speed_units);

            mapFrag = (SupportMapFragment) SupportFragmentManager.FindFragmentById(Resource.Id.map);
            mapFrag.GetMapAsync(this);
        }

        void UpdateStats()
        {
            time.Text = viewModel.ElapsedTime;
            consumption.Text = viewModel.FuelConsumption;
            consumptionUnits.Text = viewModel.FuelConsumptionUnits;
            speed.Text = viewModel.Speed;
            speedUnits.Text = viewModel.SpeedUnits;
            distanceUnits.Text = viewModel.DistanceUnits;
            distance.Text = viewModel.Distance;
        }

        void SetupMap()
        {
            if (mapFrag.View.Width == 0)
            {
                mapFrag.View.PostDelayed(SetupMap, 500);
                return;
            }
            if (viewModel.Trip == null || viewModel.Trip.Points == null || viewModel.Trip.Points.Count == 0)
                return;

            var start = viewModel.Trip.Points[0];
            var end = viewModel.Trip.Points[viewModel.Trip.Points.Count - 1];
            seekBar.Max = viewModel.Trip.Points.Count - 1;
            seekBar.ProgressChanged += SeekBar_ProgressChanged;

            var logicalDensity = Resources.DisplayMetrics.Density;
            var thicknessCar = (int) Math.Ceiling(26*logicalDensity + .5f);
            var thicknessPoints = (int) Math.Ceiling(20*logicalDensity + .5f);

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

            b = ContextCompat.GetDrawable(this, Resource.Drawable.ic_tip) as BitmapDrawable;
            finalIcon = Bitmap.CreateScaledBitmap(b.Bitmap, thicknessPoints, thicknessPoints, false);
            var poiIcon = BitmapDescriptorFactory.FromBitmap(finalIcon);
            foreach (var poi in viewModel.POIs)
            {
                var poiMarker = new MarkerOptions();
                poiMarker.SetPosition(new LatLng(poi.Latitude, poi.Longitude));
                poiMarker.SetIcon(poiIcon);
                poiMarker.Anchor(.5f, .5f);
                map.AddMarker(poiMarker);
            }


            var points = viewModel.Trip.Points.Select(s => new LatLng(s.Latitude, s.Longitude)).ToArray();
            var rectOptions = new PolylineOptions();
            rectOptions.Add(points);
            rectOptions.InvokeColor(ContextCompat.GetColor(this, Resource.Color.primary_dark));
            map.AddPolyline(rectOptions);


            map.AddMarker(startMarker);
            map.AddMarker(endMarker);

            carMarker = map.AddMarker(car);

            var boundsPoints = new LatLngBounds.Builder();
            foreach (var point in points)
                boundsPoints.Include(point);

            var bounds = boundsPoints.Build();
            map.MoveCamera(CameraUpdateFactory.NewLatLngBounds(bounds, 64));

            map.MoveCamera(CameraUpdateFactory.NewLatLng(carMarker.Position));


            seekBar.Enabled = true;
        }

        void SeekBar_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            if (carMarker == null)
                return;

            viewModel.CurrentPosition = viewModel.Trip.Points[e.Progress];

            RunOnUiThread(() =>
            {
                UpdateStats();
                carMarker.Position = new LatLng(viewModel.CurrentPosition.Latitude,
                    viewModel.CurrentPosition.Longitude);
                map.MoveCamera(CameraUpdateFactory.NewLatLng(carMarker.Position));
            });
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
                Finish();

            return base.OnOptionsItemSelected(item);
        }
    }
}