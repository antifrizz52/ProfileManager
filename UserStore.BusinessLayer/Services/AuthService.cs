using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNet.Identity;
using UserStore.BusinessLayer.DTO;
using UserStore.BusinessLayer.Infrastructure;
using UserStore.BusinessLayer.Interfaces;
using UserStore.BusinessLayer.Util;
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
                Logger.Log.WarnFormat(
                    "Создание аккаунта: отклонено. Пользователь с login={0} уже существует в системе",
                    appUserDto.Email);

                return new OperationDetails(false, "Пользователь с таким login уже существует!", "Email");
            }

            appUser = Mapper.Map<AppUserDTO, AppUser>(appUserDto);

            var result = await Database.UserManager.CreateAsync(appUser, appUserDto.Password);

            if (!result.Succeeded)
            {
                Logger.Log.ErrorFormat("Создание аккаунта: ошибка. {0}", result.Errors.FirstOrDefault());

                return new OperationDetails(false, result.Errors.FirstOrDefault(), "");
            }

            await Database.UserManager.AddToRoleAsync(appUser.Id, appUserDto.Role.ToString());
            await Database.SaveAsync();

            return new OperationDetails(true, "Регистрация прошла успешно!", "");
        }

        public async Task<AppUserDTO> FindUserByEmail(string email)
        {
            if (String.IsNullOrEmpty(email))
            {
                Logger.Log.Warn("Запрос информации об аккаунте: login пользователя не установлен");

                throw new ValidationException("Не установлен login пользователя!", "Email");
            }

            var user = await Database.UserManager.FindByNameAsync(email);

            if (user == null)
            {
                Logger.Log.WarnFormat("Запрос информации об аккаунте: пользователь с login={0} не найден", email);

                throw new ValidationException("Пользователь не найден!", "");
            }

            Mapper.Initialize(cfg => cfg.CreateMap<AppUser, AppUserDTO>());
            return Mapper.Map<AppUser, AppUserDTO>(user);
        }

        public async Task<OperationDetails> ChangePassword(int id, string oldPass, string newPass)
        {
            var result = await Database.UserManager.ChangePasswordAsync(id, oldPass, newPass);

            if (!result.Succeeded)
            {
                Logger.Log.ErrorFormat("Изменение пароля пользователя: ошибка. {0}", result.Errors.FirstOrDefault());

                return new OperationDetails(false, result.Errors.FirstOrDefault(), "");
            }

            Logger.Log.Debug("Изменение пароля пользователя: успешно");

            return new OperationDetails(true, "Пароль изменен успешно!", "");
        }

        public async Task<OperationDetails> Delete(int? id)
        {
            if(id == null)
            {
                Logger.Log.Warn("Удаление аккаунта: отклонено. Не установлен id пользователя");

                throw new ValidationException("Не установлен id пользователя", "Id");
            }

            var appUser = await Database.UserManager.FindByIdAsync(id.Value);

            if (appUser == null)
            {
                Logger.Log.WarnFormat("Удаление аккаунта: отклонено. Пользователь с id={0} не найден", id.Value);
                return new OperationDetails(false, "Пользователь не найден!", "Email");
            }

            await Database.UserManager.DeleteAsync(appUser);
            await Database.SaveAsync();

            Logger.Log.Debug("Удаление аккаунта: успешно");

            return new OperationDetails(true, "Пользователь успешно удален!", "");
        } 

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
