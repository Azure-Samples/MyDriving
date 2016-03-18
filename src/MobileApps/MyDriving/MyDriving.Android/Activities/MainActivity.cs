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

namespace MyDriving.Droid
{
    [Activity(Label = "MyDriving", Icon = "@drawable/ic_launcher",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : BaseActivity
    {
        DrawerLayout _drawerLayout;
        NavigationView _navigationView;

        int _oldPosition = -1;

        bool _shouldClose;

        protected override int LayoutResource => Resource.Layout.activity_main;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

#if !XTC
            InitializeHockeyApp();
#endif
            _drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            //Set hamburger items menu
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);

            //setup navigation view
            _navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);

            //handle navigation
            _navigationView.NavigationItemSelected += (sender, e) =>
            {
                e.MenuItem.SetChecked(true);

                ListItemClicked(e.MenuItem.ItemId);


                SupportActionBar.Title = e.MenuItem.ItemId == Resource.Id.menu_profile
                    ? Settings.Current.UserFirstName
                    : e.MenuItem.TitleFormatted.ToString();

                _drawerLayout.CloseDrawers();
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
            HockeyApp.UpdateManager.Register(this, Logger.HockeyAppAndroid);
            HockeyApp.Metrics.MetricsManager.Register(this, Application, Logger.HockeyAppAndroid);
            HockeyApp.TraceWriter.Initialize();

            AndroidEnvironment.UnhandledExceptionRaiser += (sender, args) =>
            {
                HockeyApp.TraceWriter.WriteTrace(args.Exception);
                args.Handled = true;
            };
            AppDomain.CurrentDomain.UnhandledException +=
                (sender, args) => HockeyApp.TraceWriter.WriteTrace(args.ExceptionObject);
            TaskScheduler.UnobservedTaskException += (sender, args) => HockeyApp.TraceWriter.WriteTrace(args.Exception);
        }

        void ListItemClicked(int itemId)
        {
            //this way we don't load twice, but you might want to modify this a bit.
            if (itemId == _oldPosition)
                return;
            _shouldClose = false;
            _oldPosition = itemId;

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

            _navigationView.SetCheckedItem(itemId);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    _drawerLayout.OpenDrawer(GravityCompat.Start);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnStart()
        {
            base.OnStart();
            _shouldClose = false;
        }

        public override void OnBackPressed()
        {
            if (_drawerLayout.IsDrawerOpen((int) GravityFlags.Start))
            {
                _drawerLayout.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                if (!_shouldClose)
                {
                    Toast.MakeText(this, "Press back again to exit.", ToastLength.Short).Show();
                    _shouldClose = true;
                    return;
                }
                base.OnBackPressed();
            }
        }
    }
}