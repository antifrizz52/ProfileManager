using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using UserStore.DataLayer.Entities;
using UserStore.DataLayer.Models;

namespace UserStore.DataLayer.EF
{
    public class ApplicationContext : IdentityDbContext<AppUser, AppRole, int, AppUserLogin, AppUserRole, AppUserClaim>
    {
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Department> Departments { get; set; }

        public ApplicationContext(string connectionString)
            : base(connectionString)
        {
        }
    }
}
