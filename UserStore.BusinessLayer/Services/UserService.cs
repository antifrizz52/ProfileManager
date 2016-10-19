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
                return new OperationDetails(false, "Пользователь с таким логином не найден!", "Email");

            var userProfile = Mapper.Map<UserDTO, UserProfile>(userDto);
            userProfile.Id = appUser.Id;

            Database.UserProfiles.Create(userProfile);
            await Database.SaveAsync();

            return new OperationDetails(true, "Профиль успешно создан!", "");
        }

        public UserDTO GetUser(int? id)
        {
            if (id == null)
                throw new ValidationException("Не установлено id пользователя!", "Id");

            var user = Database.UserProfiles.Get(id.Value);

            if (user == null)
                throw new ValidationException("Пользователь не найден!", "");

            Mapper.Initialize(cfg => cfg.CreateMap<UserProfile, UserDTO>());
            return Mapper.Map<UserProfile, UserDTO>(user);
        }

        public IEnumerable<UserDTO> GetUsers()
        {
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
                return new OperationDetails(false, "Профиль не найден!", "Id");

            Mapper.Initialize(cfg => cfg.CreateMap<UserDTO, UserProfile>());
            profile = Mapper.Map<UserDTO, UserProfile>(userDto);

            Database.UserProfiles.Update(profile);
            await Database.SaveAsync();

            return new OperationDetails(true, "Профиль успешно обновлен!", "");
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
