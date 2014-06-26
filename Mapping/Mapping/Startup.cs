using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Mapping.Startup))]
namespace Mapping
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
