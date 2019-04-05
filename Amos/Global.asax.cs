
using Amos.Controllers;
using Amos.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Amos
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Create initial folders
            if (!Directory.Exists(Server.MapPath("~/ZipDump")))
                Directory.CreateDirectory(Server.MapPath("~/ZipDump"));
            if (Directory.Exists(Server.MapPath("~/_FileTransfer")))
            {
                Directory.CreateDirectory(Server.MapPath("~/_FileTransfer"));
                var subjectDataFolder = Directory.CreateDirectory(Server.MapPath("~/_FileTransfer/SubjectData"));

                DirectorySecurity security = subjectDataFolder.GetAccessControl();

                SecurityIdentifier everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                security.AddAccessRule(new FileSystemAccessRule(everyone,
                                        FileSystemRights.Write,
                                        AccessControlType.Allow));

                subjectDataFolder.SetAccessControl(security);
            }

            PresentController.InitialImport();

            using (var context = new ApplicationDbContext())
            {
                if (!context.Roles.Any(r => r.Name == "Admin"))
                {
                    var store = new RoleStore<IdentityRole>(context);
                    var manager = new RoleManager<IdentityRole>(store);
                    var role = new IdentityRole { Name = "Admin" };

                    manager.Create(role);
                }

                if (!context.Users.Any(u => u.UserName == "tsmale@rktcreative.com"))
                {
                    var store = new UserStore<ApplicationUser>(context);
                    var manager = new UserManager<ApplicationUser>(store);
                    var user = new ApplicationUser { UserName = "tsmale@rktcreative.com" };

                    manager.Create(user, "Secret123!");
                    manager.AddToRole(user.Id, "Admin");
                }

                if (!context.Users.Any(u => u.UserName == "rkinnen@rktcreative.com"))
                {
                    var store = new UserStore<ApplicationUser>(context);
                    var manager = new UserManager<ApplicationUser>(store);
                    var user = new ApplicationUser { UserName = "rkinnen@rktcreative.com" };

                    manager.Create(user, "Secret123!");
                    manager.AddToRole(user.Id, "Admin");
                }

                if (!context.Users.Any(u => u.UserName == "kcomerford@rktcreative.com"))
                {
                    var store = new UserStore<ApplicationUser>(context);
                    var manager = new UserManager<ApplicationUser>(store);
                    var user = new ApplicationUser { UserName = "kcomerford@rktcreative.com" };

                    manager.Create(user, "Secret123!");
                    manager.AddToRole(user.Id, "Admin");
                }

                if (!context.Users.Any(u => u.UserName == "admin@kbrwyle.com"))
                {
                    var store = new UserStore<ApplicationUser>(context);
                    var manager = new UserManager<ApplicationUser>(store);
                    var user = new ApplicationUser { UserName = "admin@kbrwyle.com" };

                    manager.Create(user, "Secret123!");
                    manager.AddToRole(user.Id, "Admin");
                }
                context.SaveChanges();
            }
        }
    }
}
