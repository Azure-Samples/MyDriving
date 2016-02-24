using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;

using MyTrips.Droid.Fragments;
using Android.Support.V7.App;
using Android.Support.V4.View;
using Android.Support.Design.Widget;

namespace MyTrips.Droid
{
	[Activity (Label = "MyTrips.Droid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : BaseActivity
    {

        DrawerLayout drawerLayout;
        NavigationView navigationView;

        protected override int LayoutResource
        {
            get
            {
                return Resource.Layout.main;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            drawerLayout = this.FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            //Set hamburger items menu
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);

            //setup navigation view
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);

            //handle navigation
            navigationView.NavigationItemSelected += (sender, e) =>
            {
                e.MenuItem.SetChecked(true);

                ListItemClicked(e.MenuItem.ItemId);

                Snackbar.Make(drawerLayout, "You selected: " + e.MenuItem.TitleFormatted, Snackbar.LengthLong)
                    .Show();

                drawerLayout.CloseDrawers();
            };


            //if first time you will want to go ahead and click first item.
            if (savedInstanceState == null)
            {
                ListItemClicked(Resource.Id.menu_past_trips);
            }
        }

        int oldPosition = -1;
        private void ListItemClicked(int itemId)
        {
            //this way we don't load twice, but you might want to modify this a bit.
            if (itemId == oldPosition)
                return;

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
                case Resource.Id.menu_routes:
                    fragment = FragmentRecommendedRoutes.NewInstance();
                    break;
                case Resource.Id.menu_settings:
                    fragment = FragmentSettings.NewInstance();
                    break;
            }

            SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.content_frame, fragment)
                .Commit();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}


