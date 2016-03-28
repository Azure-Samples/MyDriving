// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using MyDriving.Utils;
using MyDriving.ViewModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyDriving.UWP.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsView
    {
        readonly SettingsViewModel settingsViewModel;

        public SettingsView()
        {
            InitializeComponent();
            DataContext = Settings.Current;
            settingsViewModel = new SettingsViewModel();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            App.SetTitle("SETTINGS");
            // Enable back button behavior
            SystemNavigationManager systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            systemNavigationManager.BackRequested += SystemNavigationManager_BackRequested;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            SystemNavigationManager systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            systemNavigationManager.BackRequested -= SystemNavigationManager_BackRequested;
        }

        private void SystemNavigationManager_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = TryGoBack();
            }
        }

        private bool TryGoBack()
        {
            bool navigated = false;
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
                navigated = true;
            }
            return navigated;
        }

        public void PrivacyPolicyButton_Click(object sender, RoutedEventArgs e)
        {
            settingsViewModel.OpenBrowserCommand.Execute(settingsViewModel.PrivacyPolicyUrl);
        }

        private void TermsOfUseButton_Click(object sender, RoutedEventArgs e)
        {
            settingsViewModel.OpenBrowserCommand.Execute(settingsViewModel.TermsOfUseUrl);
        }

        public void OpenSourceNoticeButton_Click(object sender, RoutedEventArgs e)
        {
            settingsViewModel.OpenBrowserCommand.Execute(settingsViewModel.OpenSourceNoticeUrl);
        }

        public void OpenSourceGitHubButton_Click(object sender, RoutedEventArgs e)
        {
            settingsViewModel.OpenBrowserCommand.Execute(settingsViewModel.SourceOnGitHubUrl);
        }

    }
}