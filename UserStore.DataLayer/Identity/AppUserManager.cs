using Microsoft.AspNet.Identity;
using UserStore.DataLayer.Models;

namespace UserStore.DataLayer.Identity
{
    public class AppUserManager : UserManager<AppUser, int>
    {
        public AppUserManager(IUserStore<AppUser, int> store)
            : base(store) { }
    }
}
