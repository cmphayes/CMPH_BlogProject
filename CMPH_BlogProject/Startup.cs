using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CMPH_BlogProject.Startup))]
namespace CMPH_BlogProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
