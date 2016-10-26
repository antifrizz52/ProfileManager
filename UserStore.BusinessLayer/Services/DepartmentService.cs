using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using UserStore.BusinessLayer.DTO;
using UserStore.BusinessLayer.Infrastructure;
using UserStore.BusinessLayer.Interfaces;
using UserStore.DataLayer.Entities;
using UserStore.DataLayer.Interfaces;

namespace UserStore.BusinessLayer.Services
{
    public class DepartmentService : IDepartmentService
    {
        private IUnitOfWork Database { get; set; }

        public DepartmentService(IUnitOfWork uow)
        {
            Database = uow;
        }

        public async Task<OperationDetails> Create(DepartmentDTO departmentDto)
        {
            Department department = Database.Departments.Find(dep => dep.Name == departmentDto.Name).FirstOrDefault();

            if (department != null)
                return new OperationDetails(false, "Отдел с таким наименованием уже существует!", "Name");

            department = new Department
            {
                Name = departmentDto.Name
            };

            Database.Departments.Create(department);
            await Database.SaveAsync();

            return new OperationDetails(true, "Отдел успешно добавлен!", "");
        }

        public DepartmentDTO GetDepartment(int? id)
        {
            if (id == null)
                throw new ValidationException("Не установлено id отдела!", "Id");

            var department = Database.Departments.Get(id.Value);

            if (department == null)
                throw new ValidationException("Отдел не найден!", "");

            Mapper.Initialize(cfg => cfg.CreateMap<Department, DepartmentDTO>());
            return Mapper.Map<Department, DepartmentDTO>(department);
        }

        public IEnumerable<DepartmentDTO> GetDepartments()
        {
            Mapper.Initialize(cfg => cfg.CreateMap<Department, DepartmentDTO>());
            return Mapper.Map<IEnumerable<Department>, IEnumerable<DepartmentDTO>>(Database.Departments.GetAll());
        }

        public IEnumerable<UserDTO> GetAssociatedUsers(int? id)
        {
            if (id == null)
                throw new ValidationException("Не установлено id отдела!", "Id");

            var users = Database.Departments.Get(id.Value).Users;

            Mapper.Initialize(cfg => cfg.CreateMap<UserProfile, UserDTO>());
            return Mapper.Map<IEnumerable<UserProfile>, IEnumerable<UserDTO>>(users);
        } 

        public async Task<OperationDetails> Update(DepartmentDTO departmentDto)
        {
            var department = Database.Departments
                .Find(dep => dep.Name == departmentDto.Name && dep.Id != departmentDto.Id)
                .FirstOrDefault();

            if (department != null)
                return new OperationDetails(false, "Отдел с таким наименованием уже существует!", "Name");

            Mapper.Initialize(cfg => cfg.CreateMap<DepartmentDTO, Department>());
            department = Mapper.Map<DepartmentDTO, Department>(departmentDto);

            Database.Departments.Update(department);
            await Database.SaveAsync();

            return new OperationDetails(true, "Отдел успешно обновлен!", "");
        }

        public async Task<OperationDetails> Delete(int? id)
        {
            if (id == null)
                throw new ValidationException("Не установлено id отдела!", "Id");

            var department = Database.Departments.Get(id.Value);

            if (department == null)
                return new OperationDetails(false, "Отдел не найден!", "");

            Database.Departments.Delete(id.Value);
            await Database.SaveAsync();

            return new OperationDetails(true, "Отдел успешно удален!", "");
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
