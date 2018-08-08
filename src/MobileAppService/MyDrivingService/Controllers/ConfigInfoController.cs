using System.Collections.Generic;
using System.Configuration;
using System.Web.Http;

namespace MyDrivingService.Controllers
{
    public class ConfigInfoController : ApiController
    {
        // TODO: Get/return configuration details (Azure, DB, etc.)

        // GET: api/ConfigInfo
        public IEnumerable<string> Get()
        {
            // TODO: Implement first!
            return new string[] { "Valid Issuer", ConfigurationManager.AppSettings["ValidIssuer"] };
        }

        // GET: api/ConfigInfo/5
        public string Get(int id)
        {
            return "value";
        }
    }
}
