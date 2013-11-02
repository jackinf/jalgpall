using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Uptime_Jalgpall.Startup))]
namespace Uptime_Jalgpall
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
