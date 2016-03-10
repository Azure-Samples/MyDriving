
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MyTrips.Droid.Controls;
using Refractored.Controls;
using MyTrips.Utils;
using Android.Util;
using Android.Content.PM;
using MyTrips.ViewModel;

namespace MyTrips.Droid.Activities
{
    [Activity(Label = "Summary", Theme="@style/MyTheme.PopupTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]   
    public class TripSummaryActivity : BaseActivity
    {
        public static TripSummaryViewModel ViewModel { get; set; }

        RatingCircle ratingCircle;

        protected override int LayoutResource
        {
            get
            {
                return Resource.Layout.activity_trip_summary;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            FindViewById<Button>(Resource.Id.button_close).Click += (sender, e) => Finish();

            var metrics = new DisplayMetrics();
            WindowManager.DefaultDisplay.GetMetrics(metrics);

            var width = metrics.WidthPixels * .85;
            var height = metrics.HeightPixels * .75;

            Window.SetLayout((int)width , (int)height);


            if (ViewModel == null)
            {
                Finish();
                return;
            }

            FindViewById<TextView>(Resource.Id.text_time).Text = ViewModel.TotalTimeDisplay;
            FindViewById<TextView>(Resource.Id.text_distance).Text = ViewModel.TotalDistanceDisplay;
            FindViewById<TextView>(Resource.Id.text_max_speed).Text = ViewModel.MaxSpeedDisplay;
            FindViewById<TextView>(Resource.Id.text_fuel_consumption).Text = ViewModel.FuelDisplay;
            FindViewById<TextView>(Resource.Id.text_hard_accelerations).Text = ViewModel.HardAccelerations.ToString();
            FindViewById<TextView>(Resource.Id.text_hard_breaks).Text = ViewModel.HardStops.ToString();

            ViewModel = null;
        }
    }
}

