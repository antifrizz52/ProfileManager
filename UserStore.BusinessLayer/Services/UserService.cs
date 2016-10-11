using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNet.Identity;
using UserStore.BusinessLayer.DTO;
using UserStore.BusinessLayer.Infrastructure;
using UserStore.BusinessLayer.Interfaces;
using UserStore.DataLayer.Entities;
using UserStore.DataLayer.Interfaces;
using UserStore.DataLayer.Models;

namespace UserStore.BusinessLayer.Services
{
    public class UserService : IUserService
    {
        IUnitOfWork Database { get; set; }

        public UserService(IUnitOfWork uow)
        {
            Database = uow;
        }

        public async Task<OperationDetails> Create(UserDTO userDto)
        {
            AppUser user = await Database.UserManager.FindByEmailAsync(userDto.Email);

            if (user == null)
            {
                user = new AppUser { Email = userDto.Email, UserName = userDto.Email };

                var result = await Database.UserManager.CreateAsync(user, userDto.Password);

                if (result.Errors.Any())
                    return new OperationDetails(false, result.Errors.FirstOrDefault(), "");

                await Database.UserManager.AddToRoleAsync(user.Id, userDto.Role);

                UserProfile userProfile = new UserProfile
                {
                    Id = user.Id,
                    Address = userDto.Address,
                    Name = userDto.Name,
                    DepartmentId = userDto.DepartmentId
                };

                Database.UserProfiles.Create(userProfile);
                await Database.SaveAsync();

                return new OperationDetails(true, "Регистрация прошла успешно!", "");
            }
            else
            {
                return new OperationDetails(false, "Пользователь с таким логином уже существует!", "Email");
            }
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
                        .ForMember(dest => dest.Department, opts => opts.MapFrom(src => src.Department.Name)));

            return Mapper.Map<IEnumerable<UserProfile>, List<UserDTO>>(users.ToList());
        }

        public async Task<ClaimsIdentity> Authenticate(UserDTO userDto)
        {
            ClaimsIdentity claim = null;

            AppUser user = await Database.UserManager.FindAsync(userDto.Email, userDto.Password);

            if (user != null)
                claim =
                    await Database.UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);

            return claim;
        }

        public async Task SetInitialData(UserDTO adminDto, List<string> roles)
        {
            foreach (string roleName in roles)
            {
                var role = await Database.RoleManager.FindByNameAsync(roleName);

                if (role == null)
                {
                    role = new AppRole { Name = roleName };
                    await Database.RoleManager.CreateAsync(role);
                }

                await Create(adminDto);
            }
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
