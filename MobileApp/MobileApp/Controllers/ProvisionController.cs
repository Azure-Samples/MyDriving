using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;

namespace MobileApp.Controllers
{
    // Use the MobileAppController attribute for each ApiController you want to use  
    // from your mobile clients 
    [MobileAppController]
    public class ProvisionController : ApiController
    {
        // GET api/values
        public string Get()
        {
            return "Hello World!";
        }

        // POST api/values
        public string Post()
        {
            return "Hello World!";
        }
    }
}
