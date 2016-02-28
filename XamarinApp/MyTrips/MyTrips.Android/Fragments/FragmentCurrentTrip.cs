using Android.OS;
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

namespace MyTrips.Droid.Fragments
{
    public class FragmentCurrentTrip : Fragment, IOnMapReadyCallback  
    {
        public static FragmentCurrentTrip NewInstance() => new FragmentCurrentTrip { Arguments = new Bundle() };


        CurrentTripViewModel viewModel;
        GoogleMap map;
        MapView mapView;
        TextView ratingText;
        RatingCircle ratingCircle;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            HasOptionsMenu = true;
            var view = inflater.Inflate(Resource.Layout.fragment_current_trip, null);

            mapView = view.FindViewById<MapView>(Resource.Id.map);
            mapView.OnCreate(savedInstanceState);
            mapView.GetMapAsync(this);

            GeolocationHelper.Current.LocationServiceConnected += OnLocationServiceConnected;
            ratingText = view.FindViewById<TextView>(Resource.Id.text_rating);
            ratingCircle = view.FindViewById<RatingCircle>(Resource.Id.rating_circle);
            ratingText.Text = "100";
            ratingCircle.Rating = 100;
            return view;
        }

        void OnLocationServiceConnected(object sender, Services.ServiceConnectedEventArgs e)
        {
            viewModel = GeolocationHelper.Current.LocationService.ViewModel;
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
            ResetTrip();

            if(carMarker == null)
                SetupMap();
        }


        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.menu_current_trip, menu);
            base.OnCreateOptionsMenu(menu, inflater);
        }

        void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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
                    map.Clear();
                    carMarker = null;
                    SetupMap();
                    break;
                case nameof(viewModel.IsBusy):
                    if (viewModel.IsBusy)
                        AndroidHUD.AndHUD.Shared.Show(Activity, "Saving Trip...", -1, AndroidHUD.MaskType.Clear);
                    else
                        AndroidHUD.AndHUD.Shared.Dismiss(Activity);
                    break;
            }
        }

        void ResetTrip()
        {
            var list = viewModel.CurrentTrip.Trail as ObservableRangeCollection<Trail>;
            list.CollectionChanged += TrailUpdated;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_take_photo:
                    if(!(viewModel?.IsBusy).GetValueOrDefault())
                        viewModel?.TakePhotoCommand.Execute(null);
                    break;
                case Resource.Id.menu_toggle:
                    if (viewModel == null || viewModel.CurrentPosition == null || viewModel.IsBusy)
                        break;
                    if (viewModel.IsRecording)
                    {
                      
                        viewModel.StopRecordingTripCommand.Execute(null);
                        AddEndMarker(new LatLng(viewModel.CurrentPosition.Latitude, viewModel.CurrentPosition.Longitude));
                    }
                    else
                    {
                        viewModel.StartRecordingTripCommand.Execute(null);
                        AddStartMarker(new LatLng(viewModel.CurrentPosition.Latitude, viewModel.CurrentPosition.Longitude));

                        UpdateCarIcon();
                    }

                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        void AddStartMarker(LatLng start)
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
        }

        void AddEndMarker(LatLng end)
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
        }

        void TrailUpdated (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Activity?.RunOnUiThread (() => {
                var item = viewModel.CurrentTrip.Trail[viewModel.CurrentTrip.Trail.Count - 1];
                if(carMarker != null)
                    UpdateMap(item);
                else
                    SetupMap();
            });
        }

        public override async void OnStart()
        {
            base.OnStart();
            await StartLocationService();
        }

        async Task StartLocationService()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                if (status != PermissionStatus.Granted)
                {
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] {Permission.Location});
                    status = results[Permission.Location];
                }

                if (status == PermissionStatus.Granted)
                {

                    if ((viewModel == null || !viewModel.IsRecording) && !GeolocationHelper.Current.IsRunning)
                        await GeolocationHelper.StartLocationService();
                    else
                        OnLocationServiceConnected(null, null);
                }
                else if(status != PermissionStatus.Unknown)
                {
                    Toast.MakeText(Activity, "Location permission is not granted, can't track location", ToastLength.Long);
                }
            }
            catch (Exception ex)
            {

            }

        }

        void UpdateCarIcon()
        {

            var logicalDensity = Resources.DisplayMetrics.Density;
            var thicknessCar = (int)Math.Ceiling(24 * logicalDensity + .5f);
            var b = ContextCompat.GetDrawable(Activity, viewModel.IsRecording ? Resource.Drawable.ic_car_red : Resource.Drawable.ic_car_blue) as BitmapDrawable;
            var finalIcon = Bitmap.CreateScaledBitmap(b.Bitmap, thicknessCar, thicknessCar, false);

            carMarker.SetIcon(BitmapDescriptorFactory.FromBitmap(finalIcon));
        }

        public override void OnStop()
        {
            base.OnStop();
            if ((viewModel?.IsRecording).GetValueOrDefault())
                return;
            
            GeolocationHelper.Current.LocationService.StopLocationUpdates();
            GeolocationHelper.StopLocationService();
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;
            if(viewModel != null)
                SetupMap();
        }

        Marker carMarker;
        PolylineOptions driveLine;
        void SetupMap()
        {
            if (map == null)
                return;

            if(mapView.Width == 0)
            {
                mapView.PostDelayed (() => { SetupMap();}, 500);
                return;
            }
            
            Trail start = null;
            if(viewModel.CurrentTrip.Trail.Count != 0)
             start = viewModel.CurrentTrip.Trail[0];
            
            UpdateCar(start == null ? null : new LatLng(start.Latitude, start.Longitude));
            var points = viewModel.CurrentTrip.Trail.Select(s => new LatLng(s.Latitude, s.Longitude)).ToArray();
            driveLine = new PolylineOptions();
            driveLine.Add(points);
            driveLine.Visible(true);
            driveLine.InvokeColor(ActivityCompat.GetColor(Activity, Resource.Color.accent));
            map.AddPolyline(driveLine);
            if (start != null)
            {
                UpdateCamera(carMarker.Position);
                AddStartMarker(new LatLng(start.Latitude, start.Longitude));
            }
        }


        bool setZoom = true;
        void UpdateMap(Trail trail)
        {
            if(map == null)
                return;
            var latlng = new LatLng(trail.Latitude, trail.Longitude);
            Activity.RunOnUiThread(() =>
            {
                UpdateCar(latlng);
                driveLine.Add(latlng);
                map.AddPolyline(driveLine);
                UpdateCamera(latlng);
            });
        }

        void UpdateCar(LatLng latlng)
        {
            if (latlng == null || map == null)
                return;

            if (carMarker == null)
            {
                var car = new MarkerOptions();
                car.SetPosition(latlng);
                car.Anchor(.5f, .5f);
                carMarker = map.AddMarker(car);
                UpdateCarIcon();

                return;
            }
            carMarker.Position = latlng;
        }
            


        void UpdateCamera(LatLng latlng)
        {
            if (map == null)
                return;
            
            if (setZoom)
            {
                map.MoveCamera(CameraUpdateFactory.NewLatLngZoom(latlng, 14));
                setZoom = false;
            }
            else
            {
                map.MoveCamera(CameraUpdateFactory.NewLatLng(latlng));
            }
         
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