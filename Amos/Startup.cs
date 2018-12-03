using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Amos.Startup))]
namespace Amos
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
