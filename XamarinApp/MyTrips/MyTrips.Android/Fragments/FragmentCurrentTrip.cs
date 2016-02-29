using Android.OS;
using Android.App;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using MyTrips.Droid.Services;
using MyTrips.ViewModel;
using MvvmHelpers;
using MyTrips.DataObjects;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using System.Linq;
using System;

using Android.Graphics;
using Android.Graphics.Drawables;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Threading.Tasks;
using MyTrips.Droid.Controls;
using MyTrips.Utils;
using Android.Support.Design.Widget;
using System.Collections.Specialized;

namespace MyTrips.Droid.Fragments
{
    public class FragmentCurrentTrip : Android.Support.V4.App.Fragment, IOnMapReadyCallback
    {
        public static FragmentCurrentTrip NewInstance() => new FragmentCurrentTrip { Arguments = new Bundle() };

        ObservableRangeCollection<Trail> trailPointList;
        CurrentTripViewModel viewModel;
        GoogleMap map;
        MapView mapView;
        TextView ratingText;
        RatingCircle ratingCircle;
        FloatingActionButton fab;
        Marker carMarker;
        Polyline driveLine;
        Color? driveLineColor = null;
        bool setZoom = true;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            HasOptionsMenu = true;
            var view = inflater.Inflate(Resource.Layout.fragment_current_trip, null);

            mapView = view.FindViewById<MapView>(Resource.Id.map);
            mapView.OnCreate(savedInstanceState);
            mapView.GetMapAsync(this);

            ratingText = view.FindViewById<TextView>(Resource.Id.text_rating);
            ratingCircle = view.FindViewById<RatingCircle>(Resource.Id.rating_circle);
            fab = view.FindViewById<FloatingActionButton>(Resource.Id.fab);

            ratingText.Text = "100";
            ratingCircle.Rating = 100;
            return view;
        }

        #region Options Menu & User Actions
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.menu_current_trip, menu);
            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_take_photo:
                    if (!(viewModel?.IsBusy).GetValueOrDefault())
                        viewModel?.TakePhotoCommand.Execute(null);
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        async void OnRecordButtonClick(object sender, EventArgs e)
        {
            if (viewModel == null || viewModel.CurrentPosition == null || viewModel.IsBusy)
                return;

            if (viewModel.IsRecording)
            {

                AddEndMarker(new LatLng(viewModel.CurrentPosition.Latitude, viewModel.CurrentPosition.Longitude));
                UpdateCarIcon(false);
                await viewModel.StopRecordingTripAsync();
            }
            else
            {
                AddStartMarker(new LatLng(viewModel.CurrentPosition.Latitude, viewModel.CurrentPosition.Longitude));
                await viewModel.StartRecordingTripAsync();
                UpdateCarIcon(true);
            }
        }
        #endregion


        void OnLocationServiceConnected(object sender, ServiceConnectedEventArgs e)
        {
            viewModel = GeolocationHelper.Current.LocationService.ViewModel;
            viewModel.PropertyChanged += OnPropertyChanged;
            ResetTrip();
        }

        void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(viewModel.CurrentPosition):
                    var latlng = new LatLng(viewModel.CurrentPosition.Latitude, viewModel.CurrentPosition.Longitude);
                    UpdateCar(latlng);
                    UpdateCamera(latlng);
                    break;
                case nameof(viewModel.CurrentTrip):
                    ResetTrip();
                    break;
                case nameof(viewModel.IsBusy):
                    if (viewModel.IsBusy)
                        AndroidHUD.AndHUD.Shared.Show(Activity, "Saving Trip...", -1, AndroidHUD.MaskType.Clear, TimeSpan.FromSeconds(30));
                    else if(AndroidHUD.AndHUD.Shared.CurrentDialog != null && AndroidHUD.AndHUD.Shared.CurrentDialog.IsShowing)
                        AndroidHUD.AndHUD.Shared.Dismiss(Activity);
                    break;
            }
        }



        void ResetTrip()
        {
            trailPointList = viewModel.CurrentTrip.Trail as ObservableRangeCollection<Trail>;
            trailPointList.CollectionChanged += OnTrailUpdated;
            carMarker = null;
            map?.Clear();
            SetupMap();
        }


        void AddStartMarker(LatLng start)
        {
            Activity?.RunOnUiThread(() =>
            {
                var logicalDensity = Resources.DisplayMetrics.Density;
                var thicknessPoints = (int)Math.Ceiling(20 * logicalDensity + .5f);

                var b = ContextCompat.GetDrawable(Activity, Resource.Drawable.ic_start_point) as BitmapDrawable;
                var finalIcon = Bitmap.CreateScaledBitmap(b.Bitmap, thicknessPoints, thicknessPoints, false);

                var startMarker = new MarkerOptions();
                startMarker.SetPosition(new LatLng(start.Latitude, start.Longitude));
                startMarker.SetIcon(BitmapDescriptorFactory.FromBitmap(finalIcon));
                startMarker.Anchor(.5f, .5f);
                map.AddMarker(startMarker);
            });
        }

        void AddEndMarker(LatLng end)
        {
            Activity?.RunOnUiThread(() =>
            {
                var logicalDensity = Resources.DisplayMetrics.Density;
                var thicknessPoints = (int)Math.Ceiling(20 * logicalDensity + .5f);
                var b = ContextCompat.GetDrawable(Activity, Resource.Drawable.ic_end_point) as BitmapDrawable;
                var finalIcon = Bitmap.CreateScaledBitmap(b.Bitmap, thicknessPoints, thicknessPoints, false);

                var endMarker = new MarkerOptions();
                endMarker.SetPosition(new LatLng(end.Latitude, end.Longitude));
                endMarker.SetIcon(BitmapDescriptorFactory.FromBitmap(finalIcon));
                endMarker.Anchor(.5f, .5f);

                map.AddMarker(endMarker);
            });
        }

        void OnTrailUpdated(object sender, NotifyCollectionChangedEventArgs e)
        {
            Activity?.RunOnUiThread(() =>
            {
                var item = viewModel.CurrentTrip.Trail[viewModel.CurrentTrip.Trail.Count - 1];
                if (carMarker != null)
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
                var thicknessCar = (int)Math.Ceiling(24 * logicalDensity + .5f);
                var b = ContextCompat.GetDrawable(Activity, recording ? Resource.Drawable.ic_car_red : Resource.Drawable.ic_car_blue) as BitmapDrawable;
                var finalIcon = Bitmap.CreateScaledBitmap(b.Bitmap, thicknessCar, thicknessCar, false);

                carMarker?.SetIcon(BitmapDescriptorFactory.FromBitmap(finalIcon));

                fab.SetImageResource(recording ? Resource.Drawable.ic_stop : Resource.Drawable.ic_start);
            });
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;
            if (viewModel != null)
                SetupMap();
        }

        void SetupMap()
        {
            if (map == null)
                return;

            if (mapView.Width == 0)
            {
                mapView.PostDelayed(() => { SetupMap(); }, 500);
                return;
            }


            Trail start = null;
            if (viewModel.CurrentTrip.Trail.Count != 0)
                start = viewModel.CurrentTrip.Trail[0];

            UpdateMap(start, false);

            if (start != null)
            {
                UpdateCamera(carMarker.Position);
                AddStartMarker(new LatLng(start.Latitude, start.Longitude));
            }
        }

        void UpdateMap(Trail trail, bool updateCamera = true)
        {
            if (map == null)
                return;
            //Get trail position or current potion to move car
            var latlng = trail == null ? 
                (viewModel?.CurrentPosition == null ?  null : new LatLng(viewModel.CurrentPosition.Latitude, viewModel.CurrentPosition.Longitude))
                : new LatLng(trail.Latitude, trail.Longitude);
            Activity?.RunOnUiThread(() =>
            {
                UpdateCar(latlng);
                driveLine?.Remove();
                var polyOptions = new PolylineOptions();
                var points = viewModel.CurrentTrip.Trail.Select(s => new LatLng(s.Latitude, s.Longitude)).ToArray();
                polyOptions.Add(points);

                if (!driveLineColor.HasValue)
                    driveLineColor = new Color(ContextCompat.GetColor(Activity, Resource.Color.accent));

                polyOptions.InvokeColor(driveLineColor.Value);
                driveLine = map.AddPolyline(polyOptions);
                if (updateCamera)
                    UpdateCamera(latlng);
            });
        }

        void UpdateCar(LatLng latlng)
        {
            if (latlng == null || map == null)
                return;
            Activity?.RunOnUiThread(() =>
            {
                if (carMarker == null)
                {
                    var car = new MarkerOptions();
                    car.SetPosition(latlng);
                    car.Anchor(.5f, .5f);
                    carMarker = map.AddMarker(car);
                    UpdateCarIcon(viewModel.IsRecording);
                    return;
                }
                carMarker.Position = latlng;
            });
        }

        void UpdateCamera(LatLng latlng)
        {
            if (map == null)
                return;
            Activity?.RunOnUiThread(() =>
            {
                if (setZoom)
                {
                    map.MoveCamera(CameraUpdateFactory.NewLatLngZoom(latlng, 14));
                    setZoom = false;
                }
                else
                {
                    map.MoveCamera(CameraUpdateFactory.NewLatLng(latlng));
                }
            });

        }

        public override void OnStop()
        {
            base.OnStop();

            GeolocationHelper.Current.LocationServiceConnected -= OnLocationServiceConnected;
            if (viewModel != null)
                viewModel.PropertyChanged -= OnPropertyChanged;
            if (trailPointList != null)
                trailPointList.CollectionChanged -= OnTrailUpdated;
            if (fab != null)
                fab.Click -= OnRecordButtonClick;
            //If we are recording then don't stop the background service
            if ((viewModel?.IsRecording).GetValueOrDefault())
                return;

            GeolocationHelper.Current.LocationService.StopLocationUpdates();
            GeolocationHelper.StopLocationService();
        }

        public override async void OnStart()
        {
            base.OnStart();

            GeolocationHelper.Current.LocationServiceConnected += OnLocationServiceConnected;

            if (fab != null)
                fab.Click += OnRecordButtonClick;
            await StartLocationService();
        }

        async Task StartLocationService()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                if (status != PermissionStatus.Granted)
                {
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Location });
                    status = results[Permission.Location];
                }

                if (status == PermissionStatus.Granted)
                {

                    if ((viewModel == null || !viewModel.IsRecording) && !GeolocationHelper.Current.IsRunning)
                        await GeolocationHelper.StartLocationService();
                    else
                        OnLocationServiceConnected(null, null);
                }
                else if (status != PermissionStatus.Unknown)
                {
                    Toast.MakeText(Activity, "Location permission is not granted, can't track location", ToastLength.Long);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Report(ex);
            }

        }

        #region MapView Lifecycle Events
        public override void OnResume()
        {
            base.OnResume();
            mapView?.OnResume();
        }

        public override void OnPause()
        {
            base.OnPause();
            mapView?.OnPause();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            mapView?.OnDestroy();
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            mapView?.OnSaveInstanceState(outState);
        }

        public override void OnLowMemory()
        {
            base.OnLowMemory();
            mapView?.OnLowMemory();
        }
        #endregion
    }
}