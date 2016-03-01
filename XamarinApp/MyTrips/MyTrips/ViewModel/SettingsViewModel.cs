using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MyTrips.Helpers;
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
            Acr.UserDialogs.IProgressDialog progress = null;


            if (CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.Android ||
                CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.iOS)
            {
                progress = Acr.UserDialogs.UserDialogs.Instance.Progress("Logging out...", show: false, maskType: Acr.UserDialogs.MaskType.Clear);
                progress.IsDeterministic = false;
            }
            try
            {
                
                if (CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.Android ||
                    CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.iOS)
                {
                    var result = await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync("Are you sure you want to logout?", "Logout?", "Yes, Logout", "Cancel");

                    if (!result)
                        return false;
                }


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
                progress?.Hide();
                progress?.Dispose();
            }

            return true;
        }
    }
}
