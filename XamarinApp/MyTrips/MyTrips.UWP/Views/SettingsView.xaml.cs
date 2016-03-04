using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MyTrips.ViewModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyTrips.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsView : Page
    {
        SettingsViewModel settingsViewModel;
        public SettingsView()
        {
            this.InitializeComponent();
            DataContext = Utils.Settings.Current;
            settingsViewModel = new SettingsViewModel();
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

        public void XamarinButton_Click(object sender, RoutedEventArgs e)
        {
            settingsViewModel.OpenBrowserCommand.Execute(settingsViewModel.XamarinUrl);
        }

        public async void Logout_Click(object sender, RoutedEventArgs e)
        {
            await settingsViewModel.StoreManager.DropEverythingAsync();
            settingsViewModel.Settings.CleanupUserProfile();

            this.Frame.Navigate(typeof(LoginView));
        }
    }
}
