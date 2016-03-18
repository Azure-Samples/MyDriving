using System;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using MyDriving.Utils;

namespace MyDriving.AzureClient
{
    public class AzureClient : IAzureClient
    {
        const string defaultMobileServiceUrl = "https://mydriving.azurewebsites.net";
        string mobileServiceUrl;
        IMobileServiceClient client;
        public IMobileServiceClient Client => client ?? (client = CreateClient());
      

        IMobileServiceClient CreateClient()
        {
            client = new MobileServiceClient(defaultMobileServiceUrl, new AuthHandler());
            client.SerializerSettings = new MobileServiceJsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                CamelCasePropertyNames = true
            };
            return client;
        }
    }
}


