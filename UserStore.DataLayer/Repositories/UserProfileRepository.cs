using System;
using System.Collections.Generic;
using System.Data.Entity;
using UserStore.DataLayer.EF;
using UserStore.DataLayer.Entities;
using UserStore.DataLayer.Interfaces;

namespace UserStore.DataLayer.Repositories
{
    public class UserProfileRepository : IRepository<UserProfile>
    {
        private ApplicationContext db;

        public UserProfileRepository(ApplicationContext context)
        {
            this.db = context;
        }

        public IEnumerable<UserProfile> GetAll()
        {
            return db.UserProfiles;
        }

        public UserProfile Get(int id)
        {
            return db.UserProfiles.Find(id);
        }

        public IEnumerable<UserProfile> Find(Func<UserProfile, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public void Create(UserProfile user)
        {
            db.UserProfiles.Add(user);
        }

        public void Update(UserProfile user)
        {
            db.Entry(user).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            UserProfile user = db.UserProfiles.Find(id);

            if (user != null)
                db.UserProfiles.Remove(user);
        }
    }
}
