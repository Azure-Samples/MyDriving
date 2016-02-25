using System;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;

namespace MyTrips.Interfaces
{
    public interface IAuthentication
    {
        Task<MobileServiceUser> LoginAsync(MobileServiceClient client, MobileServiceAuthenticationProvider provider);
        void ClearCookies();
    }
}

