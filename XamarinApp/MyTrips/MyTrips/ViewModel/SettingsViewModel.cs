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
        //Use Settings.DeviceConnectionString
        public string PrivacyPolicyUrl => "http://microsoft.com";
        public string TermsOfUseUrl => "http://microsoft.com";
        public string OpenSourceNoticeUrl =>"http://microsoft.com";
        public string SourceOnGitHubUrl => "http://microsoft.com";
        public string XamarinUrl => "http://xamarin.com";

		List<Setting> units;
		List<Setting> ioTHub;
		List<Setting> permissions;
		List<Setting> about;

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

                Settings.Logout();

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

		private Dictionary<string, List<Setting>> settingsData;
		public Dictionary<string, List<Setting>> SettingsData
		{
			get
			{
				if (settingsData == null)
				{
					var distanceSetting = new Setting { Name = "Distance", PossibleValues = new List<string> { "US/Imperial (miles)", "Metric (km)" }, Value = Settings.Current.MetricDistance ? "Metric (km)" : "US/Imperial (miles)"};
					var capacitySetting = new Setting { Name = "Capacity", PossibleValues = new List<string> { "US/Imperial (gallons)", "Metric (liters)" }, Value = Settings.Current.MetricUnits ? "Metric (liters)" : "US/Imperial (gallons)"};
					//var temperatureSetting = new Setting { Name = "Temperature", PossibleValues = new List<string> { "Fahrenheit", "Celsius" }, Value = Settings.Current.MetricTemp ? "Celsius" : "Fahrenheit" };

					distanceSetting.PropertyChanged += DistanceSetting_PropertyChanged;
					capacitySetting.PropertyChanged += CapacitySetting_PropertyChanged;
					//temperatureSetting.PropertyChanged += TemperatureSetting_PropertyChanged;
					units = new List<Setting>
					{
						distanceSetting, capacitySetting
					};

					var deviceConnectionString = new Setting { Name = "Device connection string", IsTextField = true };
					var mobileClientUrl = new Setting { Name = "Mobile client URL", IsTextField = true };

					deviceConnectionString.PropertyChanged += DeviceConnectionString_PropertyChanged;
					mobileClientUrl.PropertyChanged += MobileClientUrl_PropertyChanged;

					ioTHub = new List<Setting>
					{
						deviceConnectionString, mobileClientUrl
					};

					permissions = new List<Setting>
					{
						new Setting { Name = "Change MyTrips Permissions", IsButton = true, ButtonUrl = "Permissions" }
					};

					about = new List<Setting>
					{
						new Setting { Name = "Copyright Microsoft 2016", IsButton = true, ButtonUrl = PrivacyPolicyUrl },
						new Setting { Name = "Terms of Use", IsButton = true, ButtonUrl = TermsOfUseUrl },
						new Setting { Name = "Privacy Policy", IsButton = true, ButtonUrl = PrivacyPolicyUrl },
						new Setting { Name = "Open Source", IsButton = true, ButtonUrl = OpenSourceNoticeUrl },
						new Setting { Name = "Built in C# with Xamarin", IsButton = true, ButtonUrl = XamarinUrl },
					};

					settingsData = new Dictionary<string, List<Setting>>
					{
						{ "Units", units },
						{ "IoT Hub", ioTHub },
						{ "Permissions", permissions},
						{ "About", about }
					};
				}

				return settingsData;
			}
		}

		void DistanceSetting_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Value")
			{
				var setting = (Setting)sender;
				Settings.Current.MetricDistance = setting.Value == "Metric (km)";
			}
		}

		void CapacitySetting_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Value")
			{
				var setting = (Setting)sender;
				Settings.Current.MetricUnits = setting.Value == "Metric (liters)";
			}
		}

		/*void TemperatureSetting_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Value")
			{
				var setting = (Setting)sender;
				Settings.Current.MetricUnits = setting.Value == "Celsius";
			}
		}*/

		void DeviceConnectionString_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Value")
			{
				var setting = (Setting)sender;
				Settings.Current.DeviceConnectionString = setting.Value;
			}
		}

		void MobileClientUrl_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Value")
			{
				var setting = (Setting)sender;
				Settings.Current.MobileClientUrl = setting.Value;
			}
		}
	}
}