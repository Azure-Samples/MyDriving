using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDriving.Utils.Interfaces
{

    public static class AuthenticationManager
    {
        static readonly object authLock = new object();
        static bool isAuthenticating;

        public static bool IsAuthenticating
        {
            get
            {
                lock (authLock)
                {
                    return isAuthenticating;
                }
            }
            set
            {
                lock (authLock)
                {
                    isAuthenticating = value;
                }
            }
        }
    }

    public interface IAuthentication
    {
        Task<MobileServiceUser> LoginAsync(IMobileServiceClient client, MobileServiceAuthenticationProvider provider);

        void ClearCookies();
    }
}
