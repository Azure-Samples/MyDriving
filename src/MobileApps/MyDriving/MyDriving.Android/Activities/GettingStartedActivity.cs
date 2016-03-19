// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using MyDriving.Droid.Fragments;
using Android.Graphics;
using Android.Support.V4.Content;

namespace MyDriving.Droid.Activities
{
    [Activity(Label = "Getting Started", Icon = "@drawable/ic_launcher",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class GettingStartedActivity : BaseActivity
    {
        TabAdapter adapter;

        ViewPager pager;
        protected override int LayoutResource => Resource.Layout.activity_getting_started;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if ((int) Build.VERSION.SdkInt >= 21)
            {
                Window.SetStatusBarColor(new Color(ContextCompat.GetColor(this, Resource.Color.primary_dark)));
                Window.DecorView.SystemUiVisibility = StatusBarVisibility.Visible;
            }

            adapter = new TabAdapter(this, SupportFragmentManager);
            pager = FindViewById<ViewPager>(Resource.Id.pager);
            pager.Adapter = adapter;
            pager.OffscreenPageLimit = 3;

            SupportActionBar.Title = "Getting Started (1/5)";
            pager.PageSelected += (sender, e) => { SupportActionBar.Title = $"Getting Started ({e.Position + 1}/5)"; };

            SupportActionBar?.SetDisplayHomeAsUpEnabled(false);
            SupportActionBar?.SetDisplayShowHomeEnabled(false);
            // Create your application here
        }

        public override void OnBackPressed()
        {
        }
    }

    public class TabAdapter : FragmentStatePagerAdapter
    {
        public TabAdapter(Context context, Android.Support.V4.App.FragmentManager fm) : base(fm)
        {
        }

        public override int Count => 5;

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
            => new Java.Lang.String(string.Empty);


        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            switch (position)
            {
                case 0:
                    return FragmentGettingStarted1.NewInstance();
                case 1:
                    return FragmentGettingStarted2.NewInstance();
                case 2:
                    return FragmentGettingStarted3.NewInstance();
                case 3:
                    return FragmentGettingStarted4.NewInstance();
                case 4:
                    return FragmentGettingStarted5.NewInstance();
            }
            return null;
        }

        public override int GetItemPosition(Java.Lang.Object frag) => PositionNone;
    }
}