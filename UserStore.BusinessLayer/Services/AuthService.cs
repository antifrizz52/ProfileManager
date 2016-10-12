using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNet.Identity;
using UserStore.BusinessLayer.DTO;
using UserStore.BusinessLayer.Infrastructure;
using UserStore.BusinessLayer.Interfaces;
using UserStore.DataLayer.Interfaces;
using UserStore.DataLayer.Models;

namespace UserStore.BusinessLayer.Services
{
    class AuthService : IAuthService
    {
        IUnitOfWork Database { get; set; }

        public AuthService(IUnitOfWork uow)
        {
            Database = uow;
        }

        public async Task<ClaimsIdentity> Authenticate(AppUserDTO appUserDto)
        {
            ClaimsIdentity claim = null;

            var user = await Database.UserManager.FindAsync(appUserDto.Email, appUserDto.Password);

            if (user != null)
                claim =
                    await Database.UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);

            return claim;
        }

        public async Task<OperationDetails> Create(AppUserDTO appUserDto)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<AppUserDTO, AppUser>());

            var appUser = await Database.UserManager.FindByEmailAsync(appUserDto.Email);

            if (appUser != null)
            {
                return new OperationDetails(false, "Пользователь с таким логином уже существует!", "Email");
            }

            appUser = Mapper.Map<AppUserDTO, AppUser>(appUserDto);

            var result = await Database.UserManager.CreateAsync(appUser, appUserDto.Password);

            if (!result.Succeeded)
                return new OperationDetails(false, result.Errors.FirstOrDefault(), "");

            await Database.UserManager.AddToRoleAsync(appUser.Id, appUserDto.Role.ToString());
            await Database.SaveAsync();

            return new OperationDetails(true, "Регистрация прошла успешно!", "");
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
