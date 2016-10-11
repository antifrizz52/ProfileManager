using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using UserStore.DataLayer.EF;
using UserStore.DataLayer.Entities;
using UserStore.DataLayer.Interfaces;

namespace UserStore.DataLayer.Repositories
{
    public class DepartmentRepository : IRepository<Department>
    {
        private ApplicationContext db;

        public DepartmentRepository(ApplicationContext context)
        {
            this.db = context;
        }

        public IEnumerable<Department> GetAll()
        {
            return db.Departments;
        }

        public Department Get(int id)
        {
            return db.Departments.Find(id);
        }

        public IEnumerable<Department> Find(Func<Department, bool> predicate)
        {
            return db.Departments.Where(predicate);
        }

        public void Create(Department department)
        {
            db.Departments.Add(department);
        }

        public void Update(Department department)
        {
            var attachedEntity = db.Departments.Find(department.Id);

            if (attachedEntity != null)
            {
                var attachedEntry = db.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(department);
            }
            else
            {
                db.Entry(department).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            Department department = db.Departments.Find(id);

            if (department != null)
                db.Departments.Remove(department);
        }
    }
}
