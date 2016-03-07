using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(smarttripsService.Startup))]

namespace smarttripsService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            
            ConfigureMobileApp(app);
        }
    }
}