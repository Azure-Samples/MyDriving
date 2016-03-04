using System;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;

namespace MyTrips.AzureClient
{
    public class AzureClient : IAzureClient
    {
        IMobileServiceClient client;
        public IMobileServiceClient Client => client ?? (client = new MobileServiceClient("https://smartkar.azurewebsites.net", new AuthHandler()));
    }
}

