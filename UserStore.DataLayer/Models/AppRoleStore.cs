using Microsoft.AspNet.Identity.EntityFramework;
using UserStore.DataLayer.EF;

namespace UserStore.DataLayer.Models
{
    public class AppRoleStore : RoleStore<AppRole, int, AppUserRole>
    {
        public AppRoleStore(ApplicationContext context)
            : base(context)
        {
        }
    }
}
