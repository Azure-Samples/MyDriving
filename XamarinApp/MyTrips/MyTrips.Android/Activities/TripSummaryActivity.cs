
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

namespace MyTrips.Droid.Activities
{
    [Activity(Label = "Summary", Theme="@style/MyTheme.PopupTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]   
    public class TripSummaryActivity : BaseActivity
    {

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

            ratingCircle = FindViewById<RatingCircle>(Resource.Id.rating_circle);
            ratingCircle.Rating = 80;


            FindViewById<Button>(Resource.Id.button_close).Click += (sender, e) => Finish();

            var metrics = new DisplayMetrics();
            WindowManager.DefaultDisplay.GetMetrics(metrics);

            var width = metrics.WidthPixels * .85;
            var height = metrics.HeightPixels * .75;

            Window.SetLayout((int)width , (int)height);
        }
    }
}

