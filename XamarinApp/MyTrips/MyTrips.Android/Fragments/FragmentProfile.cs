using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using MyTrips.Droid.Controls;
using Refractored.Controls;
using MyTrips.Utils;
using MyTrips.ViewModel;


namespace MyTrips.Droid.Fragments
{
    public class FragmentProfile : Fragment
    {

        RatingCircle ratingCircle;
        CircleImageView circleImage;
        bool refresh = true;
        public static FragmentProfile NewInstance() => new FragmentProfile { Arguments = new Bundle() };
        ProfileViewModel viewModel;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.fragment_profile, null);

            ratingCircle = view.FindViewById<RatingCircle>(Resource.Id.rating_circle);
            circleImage = view.FindViewById<CircleImageView>(Resource.Id.profile_image);

            ratingCircle.Rating = 86;

            Activity.Title = Settings.Current.UserFirstName;
            viewModel = new ProfileViewModel();
            Square.Picasso.Picasso.With(Activity).Load(Settings.Current.UserProfileUrl).Into(circleImage);

            return view;
        }

        public override async void OnStart()
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
                //We did it!
            });
        }

    }
}