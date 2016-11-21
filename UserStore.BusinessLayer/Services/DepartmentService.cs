using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using UserStore.BusinessLayer.DTO;
using UserStore.BusinessLayer.Infrastructure;
using UserStore.BusinessLayer.Interfaces;
using UserStore.DataLayer.Entities;
using UserStore.DataLayer.Interfaces;
using Logger = UserStore.BusinessLayer.Util.Logger;

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
            {
                Logger.Log.WarnFormat(
                    "Создание нового отдела: отклонено. Отдел с наименованием {0} уже существует в системе",
                    department.Name);

                return new OperationDetails(false, "Отдел с таким наименованием уже существует!", "Name");
            }

            department = new Department
            {
                Name = departmentDto.Name
            };

            Database.Departments.Create(department);
            await Database.SaveAsync();

            Logger.Log.Debug("Создание нового отдела: успешно");

            return new OperationDetails(true, "Отдел успешно добавлен!", "");
        }

        public DepartmentDTO GetDepartment(int? id)
        {
            if (id == null)
            {
                throw new ValidationException("Не установлен id отдела!", "Id");
            }

            var department = Database.Departments.Get(id.Value);

            if (department == null)
            {
                Logger.Log.WarnFormat("Запрос информации по отделу: отдел с id={0} не найден", id);

                throw new ValidationException("Отдел не найден!", "");
            }

            Logger.Log.DebugFormat("Запрос информации по отделу: id={0}", id);

            Mapper.Initialize(cfg => cfg.CreateMap<Department, DepartmentDTO>());
            return Mapper.Map<Department, DepartmentDTO>(department);
        }

        public IEnumerable<DepartmentDTO> GetDepartments()
        {
            Logger.Log.Debug("Запрос списка отделов");

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
            {
                Logger.Log.WarnFormat(
                    "Обновление отдела: отклонено. Отдел с наименованием {0} уже существует в системе", 
                    department.Name);

                return new OperationDetails(false, "Отдел с таким наименованием уже существует!", "Name");
            }

            Mapper.Initialize(cfg => cfg.CreateMap<DepartmentDTO, Department>());
            department = Mapper.Map<DepartmentDTO, Department>(departmentDto);

            Database.Departments.Update(department);
            await Database.SaveAsync();

            Logger.Log.Debug("Обновление отдела: успешно");

            return new OperationDetails(true, "Отдел успешно обновлен!", "");
        }

        public async Task<OperationDetails> Delete(int? id)
        {
            if (id == null)
            {
                Logger.Log.WarnFormat("Удаление отдела: id отдела не был установлен");

                throw new ValidationException("Не установлен id отдела!", "Id");
            }

            var department = Database.Departments.Get(id.Value);

            if (department == null)
            {
                Logger.Log.WarnFormat("Удаление отдела: отдел с id={0} не найден", id);

                return new OperationDetails(false, "Отдел не найден!", "");
            }

            Database.Departments.Delete(id.Value);
            await Database.SaveAsync();

            Logger.Log.Debug("Удаление отдела: успешно");

            return new OperationDetails(true, "Отдел успешно удален!", "");
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
