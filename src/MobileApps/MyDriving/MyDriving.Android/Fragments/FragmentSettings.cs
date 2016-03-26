// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Android.OS;
using Android.Support.V7.Preferences;
using MyDriving.ViewModel;

namespace MyDriving.Droid.Fragments
{
    public class FragmentSettings : PreferenceFragmentCompat
    {
        SettingsViewModel viewModel;
        public static FragmentSettings NewInstance() => new FragmentSettings {Arguments = new Bundle()};

        public override void OnCreatePreferences(Bundle p0, string p1)
        {
            AddPreferencesFromResource(Resource.Xml.preferences);
            viewModel = new SettingsViewModel();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            FindPreference("url_privacy").PreferenceClick +=
                (sender, args) => viewModel.OpenBrowserCommand.Execute(viewModel.PrivacyPolicyUrl);
            FindPreference("url_copyright").PreferenceClick +=
                (sender, args) => viewModel.OpenBrowserCommand.Execute(viewModel.PrivacyPolicyUrl);
            FindPreference("url_xamarin").PreferenceClick +=
                (sender, args) => viewModel.OpenBrowserCommand.Execute(viewModel.XamarinUrl);
            FindPreference("url_terms").PreferenceClick +=
                (sender, args) => viewModel.OpenBrowserCommand.Execute(viewModel.TermsOfUseUrl);
            FindPreference("url_open_notice").PreferenceClick +=
                (sender, args) => viewModel.OpenBrowserCommand.Execute(viewModel.OpenSourceNoticeUrl);
            FindPreference("url_github").PreferenceClick +=
                (sender, args) => viewModel.OpenBrowserCommand.Execute(viewModel.SourceOnGitHubUrl);
            FindPreference("leave_feedback").PreferenceClick +=
                (sender, args) => HockeyApp.FeedbackManager.ShowFeedbackActivity(Activity);
        }
    }
}