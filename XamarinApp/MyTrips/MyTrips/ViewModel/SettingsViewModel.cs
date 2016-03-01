using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MyTrips.Helpers;

namespace MyTrips.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        //Use Settings.HubSetting1
        //Use Settings.HubSetting2

        string PrivacyPolicyUrl;
        string TermsOfUseUrl;
        string OpenSourceNoticeUrl;
        string SourceOnGitHubUrl;

        ICommand  logoutCommand;
        public ICommand LogoutCommand =>
            logoutCommand ?? (logoutCommand = new RelayCommand(async () => await ExecuteLogoutCommandAsync())); 

        async Task ExecuteLogoutCommandAsync()
        {
            //logout here
        }
    }
}
