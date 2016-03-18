// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Android.OS;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using MyDriving.Droid.Services;
using MyDriving.ViewModel;
using MvvmHelpers;
using MyDriving.DataObjects;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using System;
using Android.Graphics;
using Android.Graphics.Drawables;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Threading.Tasks;
using MyDriving.Droid.Activities;
using MyDriving.Utils;
using MyDriving.Droid.Helpers;
using Android.Support.Design.Widget;
using System.Collections.Specialized;
using System.Collections.Generic;
using Android.Views.Animations;
using Android.Animation;


namespace MyDriving.Droid.Fragments
{
    public class FragmentCurrentTrip : Android.Support.V4.App.Fragment, IOnMapReadyCallback
    {
        List<LatLng> _allPoints;
        Marker _carMarker;
        TextView _distance, _distanceUnits, _time, _load, _consumption, _consumptionUnits;
        Polyline _driveLine;
        Color? _driveLineColor;
        FloatingActionButton _fab;
        GoogleMap _map;
        MapView _mapView;
        bool _setZoom = true;
        LinearLayout _stats;

        ObservableRangeCollection<TripPoint> _trailPointList;
        CurrentTripViewModel _viewModel;

        public void OnMapReady(GoogleMap googleMap)
        {
            _map = googleMap;
            if (_viewModel != null)
                SetupMap();
        }

        public static FragmentCurrentTrip NewInstance() => new FragmentCurrentTrip {Arguments = new Bundle()};

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            HasOptionsMenu = true;
            var view = inflater.Inflate(Resource.Layout.fragment_current_trip, null);

            _mapView = view.FindViewById<MapView>(Resource.Id.map);
            _mapView.OnCreate(savedInstanceState);

            _fab = view.FindViewById<FloatingActionButton>(Resource.Id.fab);
            _time = view.FindViewById<TextView>(Resource.Id.text_time);
            _distance = view.FindViewById<TextView>(Resource.Id.text_distance);
            _distanceUnits = view.FindViewById<TextView>(Resource.Id.text_distance_units);
            _consumption = view.FindViewById<TextView>(Resource.Id.text_consumption);
            _consumptionUnits = view.FindViewById<TextView>(Resource.Id.text_consumption_units);
            _load = view.FindViewById<TextView>(Resource.Id.text_load);
            _stats = view.FindViewById<LinearLayout>(Resource.Id.stats);
            _stats.Visibility = ViewStates.Invisible;
            return view;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            _mapView.GetMapAsync(this);
            base.OnActivityCreated(savedInstanceState);
        }


        void StartFadeAnimation(bool fadeIn)
        {
            //handle first run
            if (!_viewModel.IsRecording && _stats.Visibility == ViewStates.Invisible)
                return;

            var start = fadeIn ? 0f : 1f;
            var end = fadeIn ? 1f : 0f;
            _stats.Alpha = fadeIn ? 0f : 1f;
            _stats.Visibility = ViewStates.Visible;


            var timerAnimator = ValueAnimator.OfFloat(start, end);
            timerAnimator.SetDuration(Java.Util.Concurrent.TimeUnit.Seconds.ToMillis(1));
            timerAnimator.SetInterpolator(new AccelerateInterpolator());
            timerAnimator.Update +=
                (sender, e) => { Activity.RunOnUiThread(() => _stats.Alpha = (float) e.Animation.AnimatedValue); };
            timerAnimator.Start();
        }

        void OnLocationServiceConnected(object sender, ServiceConnectedEventArgs e)
        {
            _viewModel = GeolocationHelper.Current.LocationService.ViewModel;
            _viewModel.PropertyChanged += OnPropertyChanged;
            ResetTrip();
        }

        void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_viewModel.CurrentPosition):
                    var latlng = _viewModel.CurrentPosition.ToLatLng();
                    UpdateCar(latlng);
                    UpdateCamera(latlng);
                    break;
                case nameof(_viewModel.CurrentTrip):
                    TripSummaryActivity.ViewModel = _viewModel.TripSummary;
                    ResetTrip();
                    StartActivity(new Android.Content.Intent(Activity, typeof (TripSummaryActivity)));
                    break;
                case "Stats":
                    UpdateStats();
                    break;
            }
        }

        void UpdateStats()
        {
            Activity?.RunOnUiThread(() =>
            {
                _time.Text = _viewModel.ElapsedTime;
                _consumption.Text = _viewModel.FuelConsumption;
                _consumptionUnits.Text = _viewModel.FuelConsumptionUnits;
                _load.Text = _viewModel.EngineLoad;
                _distanceUnits.Text = _viewModel.DistanceUnits;
                _distance.Text = _viewModel.CurrentTrip.TotalDistanceNoUnits;
            });
        }

        void ResetTrip()
        {
            _trailPointList = _viewModel.CurrentTrip.Points as ObservableRangeCollection<TripPoint>;
            _trailPointList.CollectionChanged += OnTrailUpdated;
            _carMarker = null;
            _map?.Clear();
            _allPoints?.Clear();
            _allPoints = null;
            SetupMap();
            UpdateStats();
            StartFadeAnimation(_viewModel.IsRecording);
            Activity.SupportInvalidateOptionsMenu();
        }


        void AddStartMarker(LatLng start)
        {
            Activity?.RunOnUiThread(() =>
            {
                var logicalDensity = Resources.DisplayMetrics.Density;
                var thicknessPoints = (int) Math.Ceiling(20*logicalDensity + .5f);

                var b = ContextCompat.GetDrawable(Activity, Resource.Drawable.ic_start_point) as BitmapDrawable;
                var finalIcon = Bitmap.CreateScaledBitmap(b.Bitmap, thicknessPoints, thicknessPoints, false);

                var startMarker = new MarkerOptions();
                startMarker.SetPosition(start);
                startMarker.SetIcon(BitmapDescriptorFactory.FromBitmap(finalIcon));
                startMarker.Anchor(.5f, .5f);
                _map.AddMarker(startMarker);
            });
        }

        void AddEndMarker(LatLng end)
        {
            Activity?.RunOnUiThread(() =>
            {
                var logicalDensity = Resources.DisplayMetrics.Density;
                var thicknessPoints = (int) Math.Ceiling(20*logicalDensity + .5f);
                var b = ContextCompat.GetDrawable(Activity, Resource.Drawable.ic_end_point) as BitmapDrawable;
                var finalIcon = Bitmap.CreateScaledBitmap(b.Bitmap, thicknessPoints, thicknessPoints, false);

                var endMarker = new MarkerOptions();
                endMarker.SetPosition(end);
                endMarker.SetIcon(BitmapDescriptorFactory.FromBitmap(finalIcon));
                endMarker.Anchor(.5f, .5f);

                _map.AddMarker(endMarker);
            });
        }

        void OnTrailUpdated(object sender, NotifyCollectionChangedEventArgs e)
        {
            Activity?.RunOnUiThread(() =>
            {
                var item = _viewModel.CurrentTrip.Points[_viewModel.CurrentTrip.Points.Count - 1];
                if (_carMarker != null)
                    UpdateMap(item);
                else
                    SetupMap();
            });
        }

        void UpdateCarIcon(bool recording)
        {
            Activity?.RunOnUiThread(() =>
            {
                var logicalDensity = Resources.DisplayMetrics.Density;
                var thicknessCar = (int) Math.Ceiling(26*logicalDensity + .5f);
                var b =
                    ContextCompat.GetDrawable(Activity,
                        recording ? Resource.Drawable.ic_car_red : Resource.Drawable.ic_car_blue) as BitmapDrawable;
                //var b = ContextCompat.GetDrawable(Activity, Resource.Drawable.ic_car) as BitmapDrawable;

                var finalIcon = Bitmap.CreateScaledBitmap(b.Bitmap, thicknessCar, thicknessCar, false);

                _carMarker?.SetIcon(BitmapDescriptorFactory.FromBitmap(finalIcon));

                _fab.SetImageResource(recording ? Resource.Drawable.ic_stop : Resource.Drawable.ic_start);
            });
        }

        void SetupMap()
        {
            if (_map == null)
                return;

            if (_mapView.Width == 0)
            {
                _mapView.PostDelayed(SetupMap, 500);
                return;
            }

            if (_viewModel.CurrentTrip.HasSimulatedOBDData)
            {
                var activity = (BaseActivity) Activity;
                activity.SupportActionBar.Title = "Current Trip (Sim OBD)";
            }


            TripPoint start = null;
            if (_viewModel.CurrentTrip.Points.Count != 0)
                start = _viewModel.CurrentTrip.Points[0];

            UpdateMap(start, false);

            if (start != null)
            {
                UpdateCamera(_carMarker.Position);
                AddStartMarker(start.ToLatLng());
            }
        }

        void UpdateMap(TripPoint point, bool updateCamera = true)
        {
            if (_map == null)
                return;
            //Get trail position or current potion to move car
            var latlng = point == null
                ? _viewModel?.CurrentPosition?.ToLatLng()
                : point.ToLatLng();
            Activity?.RunOnUiThread(() =>
            {
                UpdateCar(latlng);
                _driveLine?.Remove();
                var polyOptions = new PolylineOptions();

                if (_allPoints == null)
                {
                    _allPoints = _viewModel.CurrentTrip.Points.ToLatLngs();
                }
                else if (point != null)
                {
                    _allPoints.Add(point.ToLatLng());
                }

                polyOptions.Add(_allPoints.ToArray());

                if (!_driveLineColor.HasValue)
                    _driveLineColor = new Color(ContextCompat.GetColor(Activity, Resource.Color.recording_accent));

                polyOptions.InvokeColor(_driveLineColor.Value);
                _driveLine = _map.AddPolyline(polyOptions);
                if (updateCamera)
                    UpdateCamera(latlng);
            });
        }

        void UpdateCar(LatLng latlng)
        {
            if (latlng == null || _map == null)
                return;
            Activity?.RunOnUiThread(() =>
            {
                if (_carMarker == null)
                {
                    var car = new MarkerOptions();
                    car.SetPosition(latlng);
                    car.Anchor(.5f, .5f);
                    _carMarker = _map.AddMarker(car);
                    UpdateCarIcon(_viewModel.IsRecording);
                    return;
                }
                _carMarker.Position = latlng;
            });
        }

        void UpdateCamera(LatLng latlng)
        {
            if (_map == null)
                return;
            Activity?.RunOnUiThread(() =>
            {
                if (_setZoom)
                {
                    _map.MoveCamera(CameraUpdateFactory.NewLatLngZoom(latlng, 14));
                    _setZoom = false;
                }
                else
                {
                    _map.MoveCamera(CameraUpdateFactory.NewLatLng(latlng));
                }
            });
        }

        public override void OnStop()
        {
            base.OnStop();

            GeolocationHelper.Current.LocationServiceConnected -= OnLocationServiceConnected;
            if (_viewModel != null)
                _viewModel.PropertyChanged -= OnPropertyChanged;
            if (_trailPointList != null)
                _trailPointList.CollectionChanged -= OnTrailUpdated;
            if (_fab != null)
                _fab.Click -= OnRecordButtonClick;
            //If we are recording then don't stop the background service
            if ((_viewModel?.IsRecording).GetValueOrDefault())
                return;

            GeolocationHelper.Current.LocationService.StopLocationUpdates();
            GeolocationHelper.StopLocationService();
        }

        public override async void OnStart()
        {
            base.OnStart();

            GeolocationHelper.Current.LocationServiceConnected += OnLocationServiceConnected;

            if (_fab != null)
                _fab.Click += OnRecordButtonClick;
            await StartLocationService();
        }

        async Task StartLocationService()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                if (status != PermissionStatus.Granted)
                {
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                    status = results[Permission.Location];
                }

                if (status == PermissionStatus.Granted)
                {
                    if ((_viewModel == null || !_viewModel.IsRecording) && !GeolocationHelper.Current.IsRunning)
                        await GeolocationHelper.StartLocationService();
                    else
                        OnLocationServiceConnected(null, null);
                }
                else if (status != PermissionStatus.Unknown)
                {
                    Toast.MakeText(Activity, "Location permission is not granted, can't track location",
                        ToastLength.Long);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Report(ex);
            }
        }

        #region Options Menu & User Actions

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            //if((viewModel?.IsRecording).GetValueOrDefault())
            //     inflater.Inflate(Resource.Menu.menu_current_trip, menu);
            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_take_photo:
                    if (!(_viewModel?.IsBusy).GetValueOrDefault())
                        _viewModel?.TakePhotoCommand.Execute(null);
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        async void OnRecordButtonClick(object sender, EventArgs e)
        {
            if (_viewModel?.CurrentPosition == null || _viewModel.IsBusy)
                return;

            if (_viewModel.NeedSave)
            {
                await _viewModel.SaveRecordingTripAsync();
            }

            if (_viewModel.IsRecording)
            {
                if (!await _viewModel.StopRecordingTrip())
                    return;

                AddEndMarker(_viewModel.CurrentPosition.ToLatLng());
                UpdateCarIcon(false);

                var activity = (BaseActivity) Activity;
                activity.SupportActionBar.Title = "Current Trip";

                await _viewModel.SaveRecordingTripAsync();
            }
            else
            {
                if (!await _viewModel.StartRecordingTrip())
                    return;
                AddStartMarker(_viewModel.CurrentPosition.ToLatLng());

                Activity.SupportInvalidateOptionsMenu();
                UpdateCarIcon(true);
                UpdateStats();
                StartFadeAnimation(true);

                if (_viewModel.CurrentTrip.HasSimulatedOBDData)
                {
                    var activity = (BaseActivity) Activity;
                    activity.SupportActionBar.Title = "Current Trip (Sim OBD)";
                }
            }
        }

        #endregion

        #region MapView Lifecycle Events

        public override void OnResume()
        {
            base.OnResume();
            _mapView?.OnResume();
        }

        public override void OnPause()
        {
            base.OnPause();
            _mapView?.OnPause();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _mapView?.OnDestroy();
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            _mapView?.OnSaveInstanceState(outState);
        }

        public override void OnLowMemory()
        {
            base.OnLowMemory();
            _mapView?.OnLowMemory();
        }

        #endregion
    }
}