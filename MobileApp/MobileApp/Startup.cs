using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(MobileApp.Startup))]

namespace MobileApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}