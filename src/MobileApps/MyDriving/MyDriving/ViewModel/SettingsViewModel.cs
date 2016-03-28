// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using MyDriving.Helpers;
using MyDriving.Model;
using MyDriving.Utils;
using Plugin.Share;

namespace MyDriving.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        List<Setting> about;

        ICommand logoutCommand;

        ICommand openBrowserCommand;
        List<Setting> permissions;

        private Dictionary<string, List<Setting>> settingsData;

        List<Setting> units;
        //Use Settings.DeviceConnectionString
        public string PrivacyPolicyUrl => "http://aka.ms/mydriving-privacy";
        public string TermsOfUseUrl => "http://aka.ms/mydriving-eula";

        public string OpenSourceNoticeUrl => "https://github.com/Azure-Samples/MyDriving/tree/master/src/MobileApps/Licenses";

        public string SourceOnGitHubUrl => "https://github.com/Azure-Samples/MyDriving";
        public string XamarinUrl => "http://xamarin.com";

        public ICommand OpenBrowserCommand =>
            openBrowserCommand ??
            (openBrowserCommand = new RelayCommand<string>(async url => await ExecuteOpenBrowserCommandAsync(url)));

        public ICommand LogoutCommand =>
            logoutCommand ?? (logoutCommand = new RelayCommand(async () => await ExecuteLogoutCommandAsync()));

        public Dictionary<string, List<Setting>> SettingsData
        {
            get
            {
                if (settingsData == null)
                {
                    var distanceSetting = new Setting
                    {
                        Name = "Distance",
                        PossibleValues = new List<string> {"US/Imperial (miles)", "Metric (km)"},
                        Value = Settings.Current.MetricDistance ? "Metric (km)" : "US/Imperial (miles)"
                    };
                    var capacitySetting = new Setting
                    {
                        Name = "Capacity",
                        PossibleValues = new List<string> {"US/Imperial (gallons)", "Metric (liters)"},
                        Value = Settings.Current.MetricUnits ? "Metric (liters)" : "US/Imperial (gallons)"
                    };
                    //var temperatureSetting = new Setting { Name = "Temperature", PossibleValues = new List<string> { "Fahrenheit", "Celsius" }, Value = Settings.Current.MetricTemp ? "Celsius" : "Fahrenheit" };

                    distanceSetting.PropertyChanged += DistanceSetting_PropertyChanged;
                    capacitySetting.PropertyChanged += CapacitySetting_PropertyChanged;
                    //temperatureSetting.PropertyChanged += TemperatureSetting_PropertyChanged;
                    units = new List<Setting>
                    {
                        distanceSetting,
                        capacitySetting
                    };

                    permissions = new List<Setting>
                    {
                        new Setting {Name = "Change MyDriving Permissions", IsButton = true, ButtonUrl = "Permissions"}
                    };

                    about = new List<Setting>
                    {
                        new Setting {Name = "Copyright Microsoft 2016", IsButton = true, ButtonUrl = PrivacyPolicyUrl},
                        new Setting {Name = "Terms of Use", IsButton = true, ButtonUrl = TermsOfUseUrl},
                        new Setting {Name = "Privacy Policy", IsButton = true, ButtonUrl = PrivacyPolicyUrl},
                        new Setting {Name = "Open Source Notice", IsButton = true, ButtonUrl = OpenSourceNoticeUrl},
                        new Setting {Name = "Open Source on GitHub", IsButton = true, ButtonUrl = SourceOnGitHubUrl},
                        new Setting {Name = "Built in C# with Xamarin", IsButton = true, ButtonUrl = XamarinUrl},
                    };

                    settingsData = new Dictionary<string, List<Setting>>
                    {
                        {"Units", units},
                        {"Permissions", permissions},
                        {"About", about}
                    };
                }

                return settingsData;
            }
        }

        public async Task ExecuteOpenBrowserCommandAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return;

            Logger.Instance.Track("LaunchUrl");
            await CrossShare.Current.OpenBrowser(url, new Plugin.Share.Abstractions.BrowserOptions
            {
                ChromeShowTitle = true,
                ChromeToolbarColor = new Plugin.Share.Abstractions.ShareColor {A = 255, R = 96, G = 125, B = 139},
                UseSafariWebViewController = true,
                UseSafairReaderMode = false
            });
        }

        public Task<bool> ExecuteLogoutCommandAsync()
        {

            Logger.Instance.Track("Logout");
            return Task.FromResult(true);
        }

        void DistanceSetting_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                var setting = (Setting) sender;
                Settings.Current.MetricDistance = setting.Value == "Metric (km)";
            }
        }

        void CapacitySetting_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                var setting = (Setting) sender;
                Settings.Current.MetricUnits = setting.Value == "Metric (liters)";
            }
        }
    }
}
