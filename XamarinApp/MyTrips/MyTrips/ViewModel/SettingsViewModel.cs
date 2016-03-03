using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MyTrips.Helpers;
using MyTrips.Model;
using MyTrips.Utils;
using Plugin.DeviceInfo;
using Plugin.Share;

namespace MyTrips.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        //Use Settings.HubSetting1
        public string PrivacyPolicyUrl => "http://microsoft.com";
        public string TermsOfUseUrl => "http://microsoft.com";
        public string OpenSourceNoticeUrl =>"http://microsoft.com";
        public string SourceOnGitHubUrl => "http://microsoft.com";
        public string XamarinUrl => "http://xamarin.com";

        ICommand openBrowserCommand;
        public ICommand OpenBrowserCommand =>
            openBrowserCommand ?? (openBrowserCommand = new RelayCommand<string>(async (url) => await ExecuteOpenBrowserCommandAsync(url)));

        public async Task ExecuteOpenBrowserCommandAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return;

            await CrossShare.Current.OpenBrowser(url, new Plugin.Share.Abstractions.BrowserOptions
            {
                ChromeShowTitle = true,
                ChromeToolbarColor = new Plugin.Share.Abstractions.ShareColor { A = 255, R = 96, G = 125, B = 139 },
                UseSafariWebViewController = true,
                UseSafairReaderMode = false
            });
        }

        ICommand  logoutCommand;
        public ICommand LogoutCommand =>
            logoutCommand ?? (logoutCommand = new RelayCommand(async () => await ExecuteLogoutCommandAsync())); 

       
        public async Task<bool> ExecuteLogoutCommandAsync()
        {
            var progress = Acr.UserDialogs.UserDialogs.Instance.Loading("Logging out...", show: false, maskType: Acr.UserDialogs.MaskType.Clear);
            
            try
            {
                
                 var result = await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync("Are you sure you want to logout?", "Logout?", "Yes, Logout", "Cancel");

                if (!result)
                    return false;
                


                progress?.Show();
                await StoreManager.DropEverythingAsync();

                Settings.UserId = string.Empty;
                Settings.AuthToken = string.Empty;
                Settings.LoginAccount = LoginAccount.None;

            }
            catch (Exception ex)
            {
                Logger.Instance.Report(ex);
            }
            finally
            {
                progress?.Dispose();
            }

            return true;
        }

		public Dictionary<string, List<Setting>> SettingsData
		{
			get
			{
				var logOut = new List<Setting>
				{
					new Setting { Name = "Log out", IsButton = true }
				};

				var units = new List<Setting>
				{
					new Setting { Name = "Distance", PossibleValues = new List<string> { "US/Imperial (miles)", "Metric (km)"}, Value = "US/Imperial (miles)" },
					new Setting { Name = "Capacity", PossibleValues = new List<string> { "US/Imperial (gallons)", "Metric (liters)"}, Value = "US/Imperial (gallons)" },
					new Setting { Name = "Currency", PossibleValues = new List<string> { "US (Dollar)", "UK (Pound)", "Europe (Euro)"}, Value = "US (Dollar)" },
				};

				var IoTHub = new List<Setting>
				{
					new Setting { Name = "Hub Setting 1", IsTextField = true },
					new Setting { Name = "Hub Setting 2", IsTextField = true },
				};

				var permissions = new List<Setting>
				{
					new Setting { Name = "Change MyTrips Permissions", IsButton = true }
				};

				var about = new List<Setting>
				{
					new Setting { Name = "Copyright Microsoft 2016", IsButton = true },
					new Setting { Name = "Terms of Use", IsButton = true },
					new Setting { Name = "Privacy Policy", IsButton = true },
					new Setting { Name = "Open Source", IsButton = true },
					new Setting { Name = "Built in C# with Xamarin", IsButton = true },
				};

				return new Dictionary<string, List<Setting>>
				{
					{ "Log out", logOut},
					{ "Units", units },
					{ "IoT Hub", IoTHub },
					{ "Permissions", permissions},
					{ "About", about }
				};
			}
		}
    }
}
