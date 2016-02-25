using System.Threading.Tasks;
using System.Windows.Input;
using MyTrips.Utils;
using MyTrips.Helpers;
using MyTrips.Interfaces;
using Microsoft.WindowsAzure.MobileServices;
using MyTrips.DataStore.Abstractions;

namespace MyTrips.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {


        MobileServiceClient client = null;
        IAuthentication authentication;
        public LoginViewModel()
        {
            var manager = ServiceLocator.Instance.Resolve<IStoreManager>() as MyTrips.DataStore.Azure.StoreManager;

            if (manager != null)
            {
                client = MyTrips.DataStore.Azure.StoreManager.MobileService;
            }
            
            authentication = ServiceLocator.Instance.Resolve<IAuthentication>();


        }

        bool isLoggedIn;
        public bool IsLoggedIn
        {
            get { return isLoggedIn; }
            set { SetProperty(ref isLoggedIn, value); }
        }

        ICommand  loginTwitterCommand;
        public ICommand LoginTwitterCommand =>
            loginTwitterCommand ?? (loginTwitterCommand = new RelayCommand(async () => await ExecuteLoginTwitterCommandAsync())); 

        async Task ExecuteLoginTwitterCommandAsync()
        {
            if(client == null)
                return;

            Settings.LoginAccount = LoginAccount.Twitter;
            var user = await authentication.LoginAsync(client, MobileServiceAuthenticationProvider.Twitter);
            if(user == null)
            {
                Settings.LoginAccount = LoginAccount.None;
                return;
            }

            IsLoggedIn = true;
        }


        ICommand  loginMicrosoftCommand;
        public ICommand LoginMicrosoftCommand =>
            loginMicrosoftCommand ?? (loginMicrosoftCommand = new RelayCommand(async () => await ExecuteLoginMicrosoftCommandAsync())); 

        async Task ExecuteLoginMicrosoftCommandAsync()
        {
            if(client == null)
                return;

            Settings.LoginAccount = LoginAccount.Microsoft;
            var user = await authentication.LoginAsync(client, MobileServiceAuthenticationProvider.MicrosoftAccount);
            if(user == null)
            {
                Settings.LoginAccount = LoginAccount.None;
                return;
            }

            IsLoggedIn = true;
        }

        ICommand  loginFacebookCommand;
        public ICommand LoginFacebookCommand =>
            loginFacebookCommand ?? (loginFacebookCommand = new RelayCommand(async () => await ExecuteLoginFacebookCommandAsync())); 

        async Task ExecuteLoginFacebookCommandAsync()
        {
            if(client == null)
                return;

            Settings.LoginAccount = LoginAccount.Facebook;
            var user = await authentication.LoginAsync(client, MobileServiceAuthenticationProvider.Facebook);
            if(user == null)
            {
                Settings.LoginAccount = LoginAccount.None;
                return;
            }

            IsLoggedIn = true;
        }
    }
}
