using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Chearn.Startup))]
namespace Chearn
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
