using System;
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

        public async Task<AppUserDTO> FindUserByEmail(string email)
        {
            if (String.IsNullOrEmpty(email))
                throw new ValidationException("Не установлен login пользователя!", "Email");

            var user = await Database.UserManager.FindByNameAsync(email);

            if (user == null)
                throw new ValidationException("Пользователь не найден!", "");

            Mapper.Initialize(cfg => cfg.CreateMap<AppUser, AppUserDTO>());
            return Mapper.Map<AppUser, AppUserDTO>(user);
        }

        public async Task<OperationDetails> ChangePassword(int id, string oldPass, string newPass)
        {
            var result = await Database.UserManager.ChangePasswordAsync(id, oldPass, newPass);

            if (!result.Succeeded)
                return new OperationDetails(false, result.Errors.FirstOrDefault(), "");

            return new OperationDetails(true, "Пароль изменен успешно!", "");
        }

        public async Task<OperationDetails> Delete(int? id)
        {
            if(id == null)
                throw new ValidationException("Не установлено id отдела!", "Id");

            var appUser = await Database.UserManager.FindByIdAsync(id.Value);

            if (appUser == null)
                return new OperationDetails(false, "Пользователь не найден!", "Email");

            //await Database.UserManager.RemoveFromRoleAsync(appUser.Id);
            await Database.UserManager.DeleteAsync(appUser);
            await Database.SaveAsync();

            return new OperationDetails(true, "Пользователь успешно удален!", "");
        } 

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
