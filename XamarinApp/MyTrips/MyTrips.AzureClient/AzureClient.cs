using System;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using MyTrips.Utils;

namespace MyTrips.AzureClient
{
    public class AzureClient : IAzureClient
    {
        const string defaultMobileServiceUrl = "https://mydriving.azurewebsites.net";
        string mobileServiceUrl;
        IMobileServiceClient client;
        public IMobileServiceClient Client => client ?? (client = CreateClient());
      

        private IMobileServiceClient CreateClient()
        {
            if (string.IsNullOrEmpty(mobileServiceUrl))
            {
                if(string.IsNullOrEmpty(Settings.Current.MobileClientUrl))
                {
                    mobileServiceUrl = defaultMobileServiceUrl;
                    Settings.Current.MobileClientUrl = defaultMobileServiceUrl;
                }
                else
                {
                    mobileServiceUrl = Settings.Current.MobileClientUrl;
                }
            }
            client = new MobileServiceClient(mobileServiceUrl, new AuthHandler());
            client.SerializerSettings = new MobileServiceJsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                CamelCasePropertyNames = true
            };
            return client;
        }

        public void UpdateMobileServiceUrl(string url)
        {
            if(string.Compare(url, mobileServiceUrl, StringComparison.OrdinalIgnoreCase) != 0)
            {
                mobileServiceUrl = url;
                Settings.Current.MobileClientUrl = url;
                client = new MobileServiceClient(mobileServiceUrl, new AuthHandler());
            }
        }
  
    }
}


