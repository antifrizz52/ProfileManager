using Microsoft.AspNet.Identity.EntityFramework;
using UserStore.DataLayer.EF;

namespace UserStore.DataLayer.Models
{
    public class AppUserStore : UserStore<AppUser, AppRole, int, AppUserLogin, AppUserRole, AppUserClaim>
    {
        public AppUserStore(ApplicationContext context)
            : base(context)
        {
        }
    }
}
