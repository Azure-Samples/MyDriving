using System;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;

namespace MyTrips.AzureClient
{
    public interface IAzureClient
    {
        IMobileServiceClient Client { get; }

    }
}

