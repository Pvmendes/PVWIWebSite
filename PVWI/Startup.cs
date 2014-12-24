using Microsoft.Owin;
using Owin;
using PVWI;

[assembly: OwinStartup(typeof(Startup))]

namespace PVWI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
