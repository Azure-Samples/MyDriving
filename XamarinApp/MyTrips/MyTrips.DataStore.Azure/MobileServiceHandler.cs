using Microsoft.WindowsAzure.MobileServices;
using MyTrips.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrips.DataStore.Azure
{
    /// <summary>
    /// This is temporary - James is working on new mobile client class.
    /// </summary>
    public class MobileServiceHandler
    {
        private const string ConnectionStr = "https://smarttrips.azurewebsites.net";
        private static MobileServiceHandler handler;
        private MobileServiceClient client;

        public MobileServiceClient Client
        {
            get { return this.client; }
        }

        public static MobileServiceHandler GetHandler()
        {
            if (handler == null)
            {
                handler = new MobileServiceHandler();
                handler.client = new MobileServiceClient(ConnectionStr);

                if (!string.IsNullOrWhiteSpace(Settings.Current.AuthToken) && !string.IsNullOrWhiteSpace(Settings.Current.UserId))
                {
                    handler.Client.CurrentUser = new MobileServiceUser(Settings.Current.UserId);
                    handler.Client.CurrentUser.MobileServiceAuthenticationToken = Settings.Current.AuthToken;
                }
            }

            return handler;
        }
    }
}
