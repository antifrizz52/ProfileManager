using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using UserStore.BusinessLayer.DTO;
using UserStore.BusinessLayer.Infrastructure;
using UserStore.BusinessLayer.Interfaces;
using UserStore.BusinessLayer.Util;
using UserStore.DataLayer.Entities;
using UserStore.DataLayer.Interfaces;

namespace UserStore.BusinessLayer.Services
{
    public class UserService : IUserService
    {
        IUnitOfWork Database { get; set; }

        public UserService(IUnitOfWork uow)
        {
            Database = uow;
        }

        public async Task<OperationDetails> CreateProfile(UserDTO userDto)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<UserDTO, UserProfile>());

            var appUser = await Database.UserManager.FindByEmailAsync(userDto.Email);

            if (appUser == null)
            {
                Logger.Log.WarnFormat(
                    "Создание нового профиля: отклонено. Пользователь с логином {0} не найден в системе!", 
                    userDto.Email);

                return new OperationDetails(false, "Пользователь с таким логином не найден!", "Email");
            }

            var userProfile = Mapper.Map<UserDTO, UserProfile>(userDto);
            userProfile.Id = appUser.Id;

            Database.UserProfiles.Create(userProfile);
            await Database.SaveAsync();

            Logger.Log.Debug("Создание нового профиля: успешно");

            return new OperationDetails(true, "Профиль успешно создан!", "");
        }

        public UserDTO GetUser(int? id)
        {
            if (id == null)
            {
                Logger.Log.Warn("Запрос профиля пользователя: id пользователя не был установлен");

                throw new ValidationException("Не установлен id пользователя!", "Id");
            }

            var user = Database.UserProfiles.Get(id.Value);

            if (user == null)
            {
                Logger.Log.WarnFormat("Запрос профиля пользователя: пользователь с id={0} не найден", id);

                throw new ValidationException("Профиль пользователя не найден!", "");
            }

            Mapper.Initialize(cfg => cfg.CreateMap<UserProfile, UserDTO>());
            return Mapper.Map<UserProfile, UserDTO>(user);
        }

        public IEnumerable<UserDTO> GetUsers()
        {
            Logger.Log.Debug("Запрос списка профилей пользователей");

            var users = Database.UserProfiles.GetAll();

            Mapper.Initialize(
                cfg =>
                    cfg.CreateMap<UserProfile, UserDTO>()
                        .ForMember(dest => dest.DepartmentId, opts => opts.MapFrom(src => src.Department.Id)));

            return Mapper.Map<IEnumerable<UserProfile>, List<UserDTO>>(users.ToList());
        }

        public async Task<OperationDetails> Update(UserDTO userDto)
        {
            var profile = Database.UserProfiles.Find(x => x.Id == userDto.Id).FirstOrDefault();

            if (profile == null)
            {
                Logger.Log.WarnFormat("Обновление профиля пользователя: отклонено. Профиль с id={0} не найден",
                    userDto.Id);

                return new OperationDetails(false, "Профиль не найден!", "Id");
            }

            Mapper.Initialize(cfg => cfg.CreateMap<UserDTO, UserProfile>());
            profile = Mapper.Map<UserDTO, UserProfile>(userDto);

            Database.UserProfiles.Update(profile);
            await Database.SaveAsync();

            Logger.Log.Debug("Обновление профиля пользователя: успешно");

            return new OperationDetails(true, "Профиль успешно обновлен!", "");
        }

        public async Task<OperationDetails> Delete(int? id)
        {
            if (id == null)
            {
                Logger.Log.Warn("Удаление профиля пользователя: id пользователя не установлен");

                throw new ValidationException("Не установлен id пользователя!", "Id");
            }

            var user = Database.UserProfiles.Get(id.Value);

            if (user == null)
            {
                Logger.Log.WarnFormat("Удаление профиля пользователя: пользователь с id={0} не найден", id);

                throw new ValidationException("Профиль не найден!", "");
            }

            Database.UserProfiles.Delete(id.Value);
            await Database.SaveAsync();

            Logger.Log.Debug("Удаление профиля пользователя: успешно");

            return new OperationDetails(true, "Профиль успешно удален!", "");
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
