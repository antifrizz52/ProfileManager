using System;
using System.Threading.Tasks;
using UserStore.DataLayer.EF;
using UserStore.DataLayer.Entities;
using UserStore.DataLayer.Identity;
using UserStore.DataLayer.Interfaces;
using UserStore.DataLayer.Models;

namespace UserStore.DataLayer.Repositories
{
    public class IdentityUnitOfWork : IUnitOfWork
    {
        private bool disposed = false;

        private ApplicationContext db;

        private AppUserManager userManager;
        private AppRoleManager roleManager;

        private UserProfileRepository userRepository;
        private DepartmentRepository departmentRepository;

        public IdentityUnitOfWork(string connectionString)
        {
            db = new ApplicationContext(connectionString);

            userManager = new AppUserManager(new AppUserStore(db));
            roleManager = new AppRoleManager(new AppRoleStore(db));

            userRepository = new UserProfileRepository(db);
            departmentRepository = new DepartmentRepository(db);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public AppUserManager UserManager
        {
            get { return userManager; }
        }

        public AppRoleManager RoleManager
        {
            get { return roleManager; }
        }

        public IRepository<UserProfile> UserProfiles
        {
            get { return userRepository; }
        }

        public IRepository<Department> Departments
        {
            get { return departmentRepository; }
        }

        public async Task SaveAsync()
        {
            await db.SaveChangesAsync();
        }

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    userManager.Dispose();
                    roleManager.Dispose();
                    db.Dispose();
                }
                this.disposed = true;
            }
        }
    }
}
