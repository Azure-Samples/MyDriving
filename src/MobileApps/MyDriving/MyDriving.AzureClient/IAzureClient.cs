using System;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;

namespace MyDriving.AzureClient
{
    public interface IAzureClient
    {
        IMobileServiceClient Client { get; }
    }
}

