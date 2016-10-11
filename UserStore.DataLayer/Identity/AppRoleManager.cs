using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using UserStore.DataLayer.Models;

namespace UserStore.DataLayer.Identity
{
    public class AppRoleManager : RoleManager<AppRole, int>
    {
        public AppRoleManager(IRoleStore<AppRole, int> store) : base(store) { }
    }
}
