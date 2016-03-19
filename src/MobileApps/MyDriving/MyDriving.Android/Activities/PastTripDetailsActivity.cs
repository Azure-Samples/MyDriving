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

namespace MyDriving.Droid.Activities
{
    [Activity(Label = "Details", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class PastTripDetailsActivity : BaseActivity, IOnMapReadyCallback
    {
        Marker _carMarker;
        TextView _distance, _distanceUnits, _time, _speed, _speedUnits, _consumption, _consumptionUnits;
        string _id;

        GoogleMap _map;
        SupportMapFragment _mapFrag;
        SeekBar _seekBar;
        TextView _startTime, _endTime;
        PastTripsDetailViewModel _viewModel;

        protected override int LayoutResource => Resource.Layout.activity_past_trip_details;


        public async void OnMapReady(GoogleMap googleMap)
        {
            _map = googleMap;

            await _viewModel.ExecuteLoadTripCommandAsync(_id);

            _startTime.Text = _viewModel.Trip.StartTimeDisplay;
            _endTime.Text = _viewModel.Trip.EndTimeDisplay;
            SupportActionBar.Title = _viewModel.Title;
            SetupMap();
            UpdateStats();
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            if ((int) Build.VERSION.SdkInt >= 21)
            {
                Window.SetStatusBarColor(new Color(ContextCompat.GetColor(this, Resource.Color.primary_dark)));
                Window.DecorView.SystemUiVisibility = StatusBarVisibility.Visible;
            }


            _viewModel = new PastTripsDetailViewModel {Title = _id = Intent.GetStringExtra("Id")};
            _seekBar = FindViewById<SeekBar>(Resource.Id.trip_progress);
            _seekBar.Enabled = false;

            _startTime = FindViewById<TextView>(Resource.Id.text_start_time);
            _endTime = FindViewById<TextView>(Resource.Id.text_end_time);
            _startTime.Text = _endTime.Text = string.Empty;

            _time = FindViewById<TextView>(Resource.Id.text_time);
            _distance = FindViewById<TextView>(Resource.Id.text_distance);
            _distanceUnits = FindViewById<TextView>(Resource.Id.text_distance_units);
            _consumption = FindViewById<TextView>(Resource.Id.text_consumption);
            _consumptionUnits = FindViewById<TextView>(Resource.Id.text_consumption_units);
            _speed = FindViewById<TextView>(Resource.Id.text_speed);
            _speedUnits = FindViewById<TextView>(Resource.Id.text_speed_units);

            _mapFrag = (SupportMapFragment) SupportFragmentManager.FindFragmentById(Resource.Id.map);
            _mapFrag.GetMapAsync(this);
        }

        void UpdateStats()
        {
            _time.Text = _viewModel.ElapsedTime;
            _consumption.Text = _viewModel.FuelConsumption;
            _consumptionUnits.Text = _viewModel.FuelConsumptionUnits;
            _speed.Text = _viewModel.Speed;
            _speedUnits.Text = _viewModel.SpeedUnits;
            _distanceUnits.Text = _viewModel.DistanceUnits;
            _distance.Text = _viewModel.Distance;
        }

        void SetupMap()
        {
            if (_mapFrag.View.Width == 0)
            {
                _mapFrag.View.PostDelayed(SetupMap, 500);
                return;
            }
            var start = _viewModel.Trip.Points[0];
            var end = _viewModel.Trip.Points[_viewModel.Trip.Points.Count - 1];
            _seekBar.Max = _viewModel.Trip.Points.Count - 1;
            _seekBar.ProgressChanged += SeekBar_ProgressChanged;

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
            foreach (var poi in _viewModel.POIs)
            {
                var poiMarker = new MarkerOptions();
                poiMarker.SetPosition(new LatLng(poi.Latitude, poi.Longitude));
                poiMarker.SetIcon(poiIcon);
                poiMarker.Anchor(.5f, .5f);
                _map.AddMarker(poiMarker);
            }


            var points = _viewModel.Trip.Points.Select(s => new LatLng(s.Latitude, s.Longitude)).ToArray();
            var rectOptions = new PolylineOptions();
            rectOptions.Add(points);
            rectOptions.InvokeColor(ContextCompat.GetColor(this, Resource.Color.primary_dark));
            _map.AddPolyline(rectOptions);


            _map.AddMarker(startMarker);
            _map.AddMarker(endMarker);

            _carMarker = _map.AddMarker(car);

            var boundsPoints = new LatLngBounds.Builder();
            foreach (var point in points)
                boundsPoints.Include(point);

            var bounds = boundsPoints.Build();
            _map.MoveCamera(CameraUpdateFactory.NewLatLngBounds(bounds, 64));

            _map.MoveCamera(CameraUpdateFactory.NewLatLng(_carMarker.Position));


           

            _seekBar.Enabled = true;
        }

        void SeekBar_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            if (_carMarker == null)
                return;

            _viewModel.CurrentPosition = _viewModel.Trip.Points[e.Progress];

            RunOnUiThread(() =>
            {
                UpdateStats();
                _carMarker.Position = new LatLng(_viewModel.CurrentPosition.Latitude,
                    _viewModel.CurrentPosition.Longitude);
                _map.MoveCamera(CameraUpdateFactory.NewLatLng(_carMarker.Position));
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