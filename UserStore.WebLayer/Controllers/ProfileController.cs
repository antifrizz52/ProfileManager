using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using UserStore.BusinessLayer.DTO;
using UserStore.BusinessLayer.Interfaces;
using UserStore.WebLayer.Models;

namespace UserStore.WebLayer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProfileController : Controller
    {
        private IUserService userService;
        private IDepartmentService departmentService;

        public ProfileController(IUserService userServ, IDepartmentService depServ)
        {
            userService = userServ;
            departmentService = depServ;
        }

        public ActionResult Index()
        {
            var profilesDto = userService.GetUsers();

            var model = new List<DetailedUserProfileModel>();

            foreach (var item in profilesDto)
            {
                var detailedModel = new DetailedUserProfileModel();

                var profile = new UserProfileModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Email = item.Email,
                    Phone = item.Phone,
                    Address = item.Address,
                    DepartmentId = item.DepartmentId
                };

                detailedModel.UserProfile = profile;

                if (item.DepartmentId != null)
                {
                    var department = new DepartmentModel
                    {
                        Id = departmentService.GetDepartment(item.DepartmentId).Id,
                        Name = departmentService.GetDepartment(item.DepartmentId).Name
                    };

                    detailedModel.Department = department;
                }

                model.Add(detailedModel);
            }

            return View(model);
        }
    }
}