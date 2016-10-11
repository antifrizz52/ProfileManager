using Microsoft.AspNet.Identity.EntityFramework;
using UserStore.DataLayer.Entities;

namespace UserStore.DataLayer.Models
{
    public class AppUser : IdentityUser<int, AppUserLogin, AppUserRole, AppUserClaim>
    {
        public virtual UserProfile ClientProfile { get; set; }
    }
}
