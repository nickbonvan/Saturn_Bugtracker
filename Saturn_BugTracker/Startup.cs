using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Saturn_BugTracker.Startup))]
namespace Saturn_BugTracker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
