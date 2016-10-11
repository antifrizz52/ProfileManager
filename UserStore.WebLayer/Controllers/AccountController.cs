using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.Owin.Security;
using UserStore.BusinessLayer.DTO;
using UserStore.BusinessLayer.Infrastructure;
using UserStore.BusinessLayer.Interfaces;
using UserStore.WebLayer.Models;

namespace UserStore.WebLayer.Controllers
{
    public class AccountController : Controller
    {
        private IUserService userService;

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        public AccountController(IUserService serv)
        {
            userService = serv;
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                Mapper.Initialize(cfg => cfg.CreateMap<LoginModel, UserDTO>());
                var user = Mapper.Map<LoginModel, UserDTO>(model);

                ClaimsIdentity claim = await userService.Authenticate(user);

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
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                Mapper.Initialize(cfg => cfg
                        .CreateMap<RegisterModel, UserDTO>()
                        .ForMember("Role", opt => opt.UseValue("User")));

                var user = Mapper.Map<RegisterModel, UserDTO>(model);

                OperationDetails operationDetails = await userService.Create(user);

                if (operationDetails.Succeeded)
                    return View("SuccessRegister");
                else
                    ModelState.AddModelError(operationDetails.Property, operationDetails.Message);
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}