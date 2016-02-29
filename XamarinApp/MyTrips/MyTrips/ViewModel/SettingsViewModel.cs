using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MyTrips.Helpers;
using MyTrips.Utils;

namespace MyTrips.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        //Use Settings.HubSetting1

        //Use Settings.HubSetting2

       
        public async Task<bool> ExecuteLogoutCommandAsync()
        {
            var progress = Acr.UserDialogs.UserDialogs.Instance.Progress("Logging out...", show: false,  maskType: Acr.UserDialogs.MaskType.Clear);
            progress.IsDeterministic = false;
            try
            {
                var result = await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync("Are you sure you want to logout?", "Logout?", "Yes, Logout", "Cancel");

                if (!result)
                    return false;


                progress.Show();
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
                progress.Hide();
                progress.Dispose();
            }

            return true;
        }
    }
}
