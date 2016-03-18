// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
using MyDriving.ViewModel;
using Android.Graphics;
using Android.Support.V4.Content;

namespace MyDriving.Droid.Activities
{
    [Activity(Label = "Trip Summary", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class TripSummaryActivity : BaseActivity
    {
        public static TripSummaryViewModel ViewModel { get; set; }


        protected override int LayoutResource => Resource.Layout.activity_trip_summary;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if ((int) Build.VERSION.SdkInt >= 21)
            {
                Window.SetStatusBarColor(new Color(ContextCompat.GetColor(this, Resource.Color.primary_dark)));
                Window.DecorView.SystemUiVisibility = StatusBarVisibility.Visible;
            }


            if (ViewModel == null)
            {
                Finish();
                return;
            }

            SupportActionBar.SetDisplayShowHomeEnabled(false);
            SupportActionBar.SetDisplayHomeAsUpEnabled(false);


            var date = ViewModel.Date.ToLocalTime();
            FindViewById<TextView>(Resource.Id.text_time).Text = ViewModel.TotalTimeDisplay;
            FindViewById<TextView>(Resource.Id.text_date).Text = date.ToString("M") + " " + date.ToString("t");
            FindViewById<TextView>(Resource.Id.text_distance).Text = ViewModel.TotalDistanceDisplay;
            FindViewById<TextView>(Resource.Id.text_max_speed).Text = ViewModel.MaxSpeedDisplay;
            FindViewById<TextView>(Resource.Id.text_fuel_consumption).Text = ViewModel.FuelDisplay;
            FindViewById<TextView>(Resource.Id.text_hard_accelerations).Text = ViewModel.HardAccelerations.ToString();
            FindViewById<TextView>(Resource.Id.text_hard_breaks).Text = ViewModel.HardStops.ToString();

            ViewModel = null;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_summary, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_close:
                    Finish();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}