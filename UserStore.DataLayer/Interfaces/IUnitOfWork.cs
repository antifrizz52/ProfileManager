using System;
using System.Threading.Tasks;
using UserStore.DataLayer.Entities;
using UserStore.DataLayer.Identity;

namespace UserStore.DataLayer.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        AppUserManager UserManager { get; }
        AppRoleManager RoleManager { get; }

        IRepository<UserProfile> UserProfiles { get; }
        IRepository<Department> Departments { get; }

        Task SaveAsync();
    }
}
