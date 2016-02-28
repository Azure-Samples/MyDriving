using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using MyTrips.Droid.Controls;
using Refractored.Controls;


namespace MyTrips.Droid.Fragments
{
    public class FragmentProfile : Fragment
    {

        RatingCircle ratingCircle;
        CircleImageView circleImage;
        public static FragmentProfile NewInstance() => new FragmentProfile { Arguments = new Bundle() };

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.fragment_profile, null);

            ratingCircle = view.FindViewById<RatingCircle>(Resource.Id.rating_circle);
            circleImage = view.FindViewById<CircleImageView>(Resource.Id.profile_image);

            ratingCircle.Rating = 86;

            Koush.UrlImageViewHelper.SetUrlDrawable (circleImage, "http://refractored.com/images/Scott.png");

            return view;
        }

    }
}