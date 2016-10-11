using Microsoft.AspNet.Identity.EntityFramework;

namespace UserStore.DataLayer.Models
{
    public class AppRole : IdentityRole<int, AppUserRole>
    {
        public AppRole() { }

        public AppRole(string name) { Name = name; }
    }
}
