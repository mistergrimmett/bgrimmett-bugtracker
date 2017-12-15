using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(bgrimmett_bugtracker.Startup))]
namespace bgrimmett_bugtracker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
