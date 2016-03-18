using Android.OS;
using Android.Support.V7.Preferences;
using Android.Content;
using Android.Views;
using Android.App;
using MyTrips.ViewModel;
using MyTrips.Droid.Activities;

namespace MyTrips.Droid.Fragments
{
    public class FragmentSettings : PreferenceFragmentCompat
    {

        SettingsViewModel viewModel;
        public static FragmentSettings NewInstance() => new FragmentSettings { Arguments = new Bundle() };

        public override void OnCreatePreferences(Bundle p0, string p1)
        {
            AddPreferencesFromResource(Resource.Xml.preferences);
            viewModel = new SettingsViewModel();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            FindPreference("url_privacy").PreferenceClick += (sender, args) => viewModel.OpenBrowserCommand.Execute(viewModel.PrivacyPolicyUrl);
            FindPreference("url_copyright").PreferenceClick += (sender, args) => viewModel.OpenBrowserCommand.Execute(viewModel.PrivacyPolicyUrl);
            FindPreference("url_xamarin").PreferenceClick += (sender, args) => viewModel.OpenBrowserCommand.Execute(viewModel.XamarinUrl);
            FindPreference("url_terms").PreferenceClick += (sender, args) => viewModel.OpenBrowserCommand.Execute(viewModel.TermsOfUseUrl);
            FindPreference("url_open_notice").PreferenceClick += (sender, args) => viewModel.OpenBrowserCommand.Execute(viewModel.OpenSourceNoticeUrl);
            FindPreference("url_github").PreferenceClick += (sender, args) => viewModel.OpenBrowserCommand.Execute(viewModel.SourceOnGitHubUrl);
        }
    }
}