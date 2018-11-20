using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Amos.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<Section> Sections { get; set; }
        public virtual DbSet<Chapter> Chapters { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        //public virtual DbSet<BookPage> BookPages { get; set; }
        public virtual DbSet<AmosFile> AmosFiles { get; set; }
        public virtual DbSet<UserTracker> UserTrackers { get; set; }
        public virtual DbSet<ScheduledJobTracker> ScheduledJobTrackers { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}