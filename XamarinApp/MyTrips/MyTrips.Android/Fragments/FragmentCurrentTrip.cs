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

namespace MyTrips.Droid.Fragments
{
    public class FragmentCurrentTrip : Fragment, IOnMapReadyCallback  
    {
        TextView latText, longText, altText;

        public static FragmentCurrentTrip NewInstance() => new FragmentCurrentTrip { Arguments = new Bundle() };


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
                    viewModel.IsRecording = true;
                    var list = viewModel.CurrentTrip.Trail as ObservableRangeCollection<Trail>;
                    list.CollectionChanged += TrailUpdated;

                    if(carMarker == null)
                        SetupMap();
                };
            latText = view.FindViewById<TextView>(Resource.Id.lat);
            longText = view.FindViewById<TextView>(Resource.Id.longx);
            altText = view.FindViewById<TextView>(Resource.Id.alt);

            return view;
        }

        void TrailUpdated (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Activity?.RunOnUiThread (() => {
                var item = viewModel.CurrentTrip.Trail[viewModel.CurrentTrip.Trail.Count - 1];
                latText.Text = $"Latitude: {item.Latitude}";
                longText.Text = $"Longitude: {item.Longitude}";
                altText.Text = $"Altitude: {item.TimeStamp}";

                if(carMarker != null)
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

        Marker carMarker;
        PolylineOptions driveLine;
        void SetupMap()
        {
            if (map == null)
                return;
            
            if ((viewModel?.CurrentTrip?.Trail?.Count).GetValueOrDefault() == 0)
                return;

            if(mapView.Width == 0)
            {
                mapView.PostDelayed (() => { SetupMap();}, 500);
                return;
            }
            
            var start = viewModel.CurrentTrip.Trail[0];
            var startPoint = new LatLng(start.Latitude, start.Longitude);


            var logicalDensity = Resources.DisplayMetrics.Density;
            var thicknessCar = (int)Math.Ceiling(24 * logicalDensity + .5f);

            var b = ContextCompat.GetDrawable(Activity, Resource.Drawable.ic_car_red) as BitmapDrawable;
            var finalIcon = Bitmap.CreateScaledBitmap(b.Bitmap, thicknessCar, thicknessCar, false);

            var car = new MarkerOptions();
            car.SetPosition(new LatLng(start.Latitude, start.Longitude));
            car.SetIcon(BitmapDescriptorFactory.FromBitmap(finalIcon));
            car.Anchor(.5f, .5f);
            carMarker = map.AddMarker(car);

            var points = viewModel.CurrentTrip.Trail.Select(s => new LatLng(s.Latitude, s.Longitude)).ToArray();
            driveLine = new PolylineOptions();
            driveLine.Add(points);
            driveLine.Visible(true);
            driveLine.InvokeZIndex(30f);
            driveLine.InvokeColor(ActivityCompat.GetColor(Activity, Resource.Color.accent));
            map.AddPolyline(driveLine);
            UpdateCamera(startPoint);
        }


        bool setZoom = true;
        void UpdateMap(Trail trail)
        {
            if(map == null)
                return;
            var latlng = new LatLng(trail.Latitude, trail.Longitude);
            Activity.RunOnUiThread(() =>
            {
                carMarker.Position = latlng;
                driveLine.Add(latlng);
                map.AddPolyline(driveLine);
                UpdateCamera(latlng);
            });
        }


        void UpdateCamera(LatLng latlng)
        {

            var current = viewModel.CurrentTrip.Trail[viewModel.CurrentTrip.Trail.Count - 1];

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