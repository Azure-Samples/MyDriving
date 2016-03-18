using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(MyDrivingService.Startup))]

namespace MyDrivingService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}