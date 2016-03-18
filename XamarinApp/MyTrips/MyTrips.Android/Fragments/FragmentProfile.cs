using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using MyTrips.Droid.Controls;
using Refractored.Controls;
using MyTrips.Utils;
using MyTrips.ViewModel;
using Android.Widget;

namespace MyTrips.Droid.Fragments
{
    public class FragmentProfile : Fragment
    {

        RatingCircle ratingCircle;
        CircleImageView circleImage;
        bool refresh = true;
        public static FragmentProfile NewInstance() => new FragmentProfile { Arguments = new Bundle() };
        ProfileViewModel viewModel;

        LinearLayout profileAll;

        TextView distance, maxSpeed, time, stops, accelerations, trips, fuelUsed, distanceUnits, profileRating, profileGreat, profileBetter;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.fragment_profile, null);

            ratingCircle = view.FindViewById<RatingCircle>(Resource.Id.rating_circle);
            circleImage = view.FindViewById<CircleImageView>(Resource.Id.profile_image);

           

            viewModel = new ProfileViewModel();
            Square.Picasso.Picasso.With(Activity).Load(Settings.Current.UserProfileUrl).Into(circleImage);

            trips = view.FindViewById<TextView>(Resource.Id.text_trips);
            time = view.FindViewById<TextView>(Resource.Id.text_time);
            distance = view.FindViewById<TextView>(Resource.Id.text_distance);
            maxSpeed = view.FindViewById<TextView>(Resource.Id.text_max_speed);
            fuelUsed = view.FindViewById<TextView>(Resource.Id.text_fuel_consumption);
            accelerations = view.FindViewById<TextView>(Resource.Id.text_hard_accelerations);
            stops = view.FindViewById<TextView>(Resource.Id.text_hard_breaks);
            profileAll = view.FindViewById<LinearLayout>(Resource.Id.text_profile_all);

            profileGreat = view.FindViewById<TextView>(Resource.Id.text_profile_great);
            profileBetter = view.FindViewById<TextView>(Resource.Id.text_profile_better);
            profileRating = view.FindViewById<TextView>(Resource.Id.text_profile_rating);
            profileAll.Visibility = ViewStates.Invisible;
            UpdateUI();
            return view;
        }

        public override void OnStart()
        {
            base.OnStart();

            if (refresh)
            {
                refresh = false;
                viewModel.UpdateProfileAsync().ContinueWith((t)=>UpdateUI());
            }
        }

        void UpdateUI()
        {
            Activity.RunOnUiThread(() =>
            {
                trips.Text = viewModel.TotalTrips.ToString();
                time.Text = viewModel.TotalTimeDisplay;
                distance.Text = viewModel.TotalDistanceDisplay;
                maxSpeed.Text = viewModel.MaxSpeedDisplay;
                fuelUsed.Text = viewModel.FuelDisplay;
                accelerations.Text = viewModel.HardAccelerations.ToString();
                stops.Text = viewModel.HardStops.ToString();
                ratingCircle.Rating = viewModel.DrivingSkills;

                profileGreat.Text = $"Driving Skills: {viewModel.DrivingSkillsPlacementBucket.Description}";
                profileBetter.Text = $"Better than {viewModel.DrivingSkills.ToString()}% of drivers";
                profileRating.Text = $"{viewModel.DrivingSkills.ToString()}%";


                profileAll.Visibility = ViewStates.Visible;
            });
        }

    }
}