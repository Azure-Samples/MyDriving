// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using MyDriving.Droid.Controls;
using Refractored.Controls;
using MyDriving.Utils;
using MyDriving.ViewModel;
using Android.Widget;

namespace MyDriving.Droid.Fragments
{
    public class FragmentProfile : Fragment
    {
        CircleImageView _circleImage;

        TextView _distance,
            _maxSpeed,
            _time,
            _stops,
            _accelerations,
            _trips,
            _fuelUsed,
            _distanceUnits,
            _profileRating,
            _profileGreat,
            _profileBetter;

        LinearLayout _profileAll;

        RatingCircle _ratingCircle;
        bool _refresh = true;
        ProfileViewModel _viewModel;
        public static FragmentProfile NewInstance() => new FragmentProfile {Arguments = new Bundle()};

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.fragment_profile, null);

            _ratingCircle = view.FindViewById<RatingCircle>(Resource.Id.rating_circle);
            _circleImage = view.FindViewById<CircleImageView>(Resource.Id.profile_image);


            _viewModel = new ProfileViewModel();
            Square.Picasso.Picasso.With(Activity).Load(Settings.Current.UserProfileUrl).Into(_circleImage);

            _trips = view.FindViewById<TextView>(Resource.Id.text_trips);
            _time = view.FindViewById<TextView>(Resource.Id.text_time);
            _distance = view.FindViewById<TextView>(Resource.Id.text_distance);
            _maxSpeed = view.FindViewById<TextView>(Resource.Id.text_max_speed);
            _fuelUsed = view.FindViewById<TextView>(Resource.Id.text_fuel_consumption);
            _accelerations = view.FindViewById<TextView>(Resource.Id.text_hard_accelerations);
            _stops = view.FindViewById<TextView>(Resource.Id.text_hard_breaks);
            _profileAll = view.FindViewById<LinearLayout>(Resource.Id.text_profile_all);

            _profileGreat = view.FindViewById<TextView>(Resource.Id.text_profile_great);
            _profileBetter = view.FindViewById<TextView>(Resource.Id.text_profile_better);
            _profileRating = view.FindViewById<TextView>(Resource.Id.text_profile_rating);
            _profileAll.Visibility = ViewStates.Invisible;
            UpdateUI();
            return view;
        }

        public override void OnStart()
        {
            base.OnStart();

            if (_refresh)
            {
                _refresh = false;
                _viewModel.UpdateProfileAsync().ContinueWith(t => UpdateUI());
            }
        }

        void UpdateUI()
        {
            Activity.RunOnUiThread(() =>
            {
                _trips.Text = _viewModel.TotalTrips.ToString();
                _time.Text = _viewModel.TotalTimeDisplay;
                _distance.Text = _viewModel.TotalDistanceDisplay;
                _maxSpeed.Text = _viewModel.MaxSpeedDisplay;
                _fuelUsed.Text = _viewModel.FuelDisplay;
                _accelerations.Text = _viewModel.HardAccelerations.ToString();
                _stops.Text = _viewModel.HardStops.ToString();
                _ratingCircle.Rating = _viewModel.DrivingSkills;

                _profileGreat.Text = $"Driving Skills: {_viewModel.DrivingSkillsPlacementBucket.Description}";
                _profileBetter.Text = $"Better than {_viewModel.DrivingSkills.ToString()}% of drivers";
                _profileRating.Text = $"{_viewModel.DrivingSkills.ToString()}%";


                _profileAll.Visibility = ViewStates.Visible;
            });
        }
    }
}