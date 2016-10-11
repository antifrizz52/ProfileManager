using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using AutoMapper;
using UserStore.BusinessLayer.DTO;
using UserStore.BusinessLayer.Interfaces;
using UserStore.BusinessLayer.Services;
using UserStore.WebLayer.Models;

namespace UserStore.WebLayer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProfileController : Controller
    {
        private IUserService userService;

        public ProfileController(IUserService service)
        {
            userService = service;
        }

        // GET: Profile
        public ActionResult Index()
        {
            var profilesDto = userService.GetUsers();

            Mapper.Initialize(cfg => cfg.CreateMap<UserDTO, UserProfileModel>());
            var profiles = Mapper.Map<IEnumerable<UserDTO>, List<UserProfileModel>>(profilesDto);

            return View(profiles);
        }
    }
}