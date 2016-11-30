using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.Owin.Security;
using UserStore.BusinessLayer.DTO;
using UserStore.BusinessLayer.Infrastructure;
using UserStore.BusinessLayer.Interfaces;
using UserStore.BusinessLayer.Util;
using UserStore.WebLayer.Models;

namespace UserStore.WebLayer.Controllers
{
    public class AccountController : Controller
    {
        private IUserService userService;
        private IAuthService authService;

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        public AccountController(IUserService userSrv, IAuthService authSrv)
        {
            userService = userSrv;
            authService = authSrv;
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid) return View(model);

            Mapper.Initialize(cfg => cfg.CreateMap<LoginModel, AppUserDTO>());
            var user = Mapper.Map<LoginModel, AppUserDTO>(model);

            ClaimsIdentity claim = await authService.Authenticate(user);

            if (claim == null)
            {
                ModelState.AddModelError("", "Неверный логин или пароль!");
            }
            else
            {
                AuthenticationManager.SignOut();
                AuthenticationManager.SignIn(new AuthenticationProperties
                {
                    IsPersistent = true
                }, claim);

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid) return View(model);

            Mapper.Initialize(cfg =>
            {
                cfg
                    .CreateMap<RegisterModel, AppUserDTO>()
                    .ForMember("Role", opt => opt.UseValue(BusinessLayer.DTO.Role.User))
                    .ForMember("UserName", opt => opt.MapFrom("Email"));

                cfg.CreateMap<RegisterModel, UserDTO>();
            });

            var appUserDto = Mapper.Map<RegisterModel, AppUserDTO>(model);
            var userDto = Mapper.Map<RegisterModel, UserDTO>(model);

            OperationDetails operationDetails;

            try
            {
                operationDetails = await authService.Create(appUserDto);
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Регистрация аккаунта: ошибка", ex);
                throw;
            }

            if (!operationDetails.Succeeded)
                throw new ValidationException(operationDetails.Message, operationDetails.Property);

            try
            {
                operationDetails = await userService.CreateProfile(userDto);
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Создание профиля пользователя: ошибка", ex);
            }

            if (operationDetails.Succeeded)
            {
                return View("SuccessRegister");
            }

            throw new ValidationException(operationDetails.Message, operationDetails.Property);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(PasswordModel model)
        {
            AppUserDTO appUser;

            if (!ModelState.IsValid) return View(model);

            try
            {
                appUser = await authService.FindUserByEmail(User.Identity.Name);
            }
            catch (ValidationException ex)
            {
                Logger.Log.Warn("Изменение пароля пользователя: предупреждение", ex);
                throw;
            }
            catch (Exception ex)
            {
                Logger.Log.Warn("Изменение пароля пользователя: ошибка", ex);
                throw;
            }

            OperationDetails operationDetails;

            try
            {
                operationDetails =await authService.ChangePassword(appUser.Id, model.CurrentPassword, model.NewPassword);
            }
            catch (ValidationException ex)
            {
                Logger.Log.Warn("Изменение пароля пользователя: предупреждение", ex);
                throw;
            }
            catch (Exception ex)
            {
                Logger.Log.Warn("Изменение пароля пользователя: ошибка", ex);
                throw;
            }

            if (operationDetails.Succeeded)
                return View("SuccessPassChange");

            throw new ValidationException(operationDetails.Message, operationDetails.Property);
        }

        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}