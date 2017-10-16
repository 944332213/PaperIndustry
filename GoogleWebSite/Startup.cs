using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GoogleWebSite.Startup))]
namespace GoogleWebSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
