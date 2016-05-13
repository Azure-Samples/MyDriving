// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using MyDriving.Droid.Fragments;
using Android.Support.V4.View;
using Android.Support.Design.Widget;
using MyDriving.Utils;
using Android.Runtime;
using System;
using System.Threading.Tasks;
using HockeyApp;


namespace MyDriving.Droid
{
    [Activity(Label = "MyDriving", Icon = "@drawable/ic_launcher",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : BaseActivity
    {
        DrawerLayout drawerLayout;
        NavigationView navigationView;

        int oldPosition = -1;

        bool shouldClose;

        protected override int LayoutResource => Resource.Layout.activity_main;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

#if !XTC
            InitializeHockeyApp();
#endif
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            //Set hamburger items menu
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);

            //setup navigation view
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);

            //handle navigation
            navigationView.NavigationItemSelected += (sender, e) =>
            {
                e.MenuItem.SetChecked(true);

                ListItemClicked(e.MenuItem.ItemId);


                SupportActionBar.Title = e.MenuItem.ItemId == Resource.Id.menu_profile
                    ? Settings.Current.UserFirstName
                    : e.MenuItem.TitleFormatted.ToString();

                drawerLayout.CloseDrawers();
            };

            if (Intent.GetBooleanExtra("tracking", false))
            {
                ListItemClicked(Resource.Id.menu_current_trip);
                SupportActionBar.Title = "Current Trip";
                return;
            }

            //if first time you will want to go ahead and click first item.
            if (bundle == null)
            {
                ListItemClicked(Resource.Id.menu_current_trip);
                SupportActionBar.Title = "Current Trip";
            }
        }

        void InitializeHockeyApp()
        {
            if (string.IsNullOrWhiteSpace(Logger.HockeyAppAndroid))
                return;

            HockeyApp.CrashManager.Register(this, Logger.HockeyAppAndroid);
            HockeyApp.Metrics.MetricsManager.Register(this, Application, Logger.HockeyAppAndroid);
            HockeyApp.Metrics.MetricsManager.EnableUserMetrics();

            CheckForUpdates();

        }

        void CheckForUpdates()
        {
            // Remove this for store builds!
            UpdateManager.Register(this, Logger.HockeyAppAndroid);
        }

        void UnregisterManagers()
        {
            UpdateManager.Unregister();
        }

        protected override void OnPause()
        {
            base.OnPause();

            UnregisterManagers();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            UnregisterManagers();
        }


        void ListItemClicked(int itemId)
        {
            //this way we don't load twice, but you might want to modify this a bit.
            if (itemId == oldPosition)
                return;
            shouldClose = false;
            oldPosition = itemId;

            Android.Support.V4.App.Fragment fragment = null;
            switch (itemId)
            {
                case Resource.Id.menu_past_trips:
                    fragment = FragmentPastTrips.NewInstance();
                    break;
                case Resource.Id.menu_current_trip:
                    fragment = FragmentCurrentTrip.NewInstance();
                    break;
                case Resource.Id.menu_profile:
                    fragment = FragmentProfile.NewInstance();
                    break;
                case Resource.Id.menu_settings:
                    fragment = FragmentSettings.NewInstance();
                    break;
            }


            SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.content_frame, fragment)
                .Commit();

            navigationView.SetCheckedItem(itemId);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    drawerLayout.OpenDrawer(GravityCompat.Start);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnStart()
        {
            base.OnStart();
            shouldClose = false;
        }

        public override void OnBackPressed()
        {
            if (drawerLayout.IsDrawerOpen((int) GravityFlags.Start))
            {
                drawerLayout.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                if (!shouldClose)
                {
                    Toast.MakeText(this, "Press back again to exit.", ToastLength.Short).Show();
                    shouldClose = true;
                    return;
                }
                base.OnBackPressed();
            }
        }
    }
}