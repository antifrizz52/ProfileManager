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

            foreach (var userDto in profilesDto)
            {
                var detailedModel = new DetailedUserProfileModel();

                var departmentDto = userDto.DepartmentId == null 
                    ? null 
                    : departmentService.GetDepartment(userDto.DepartmentId);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<UserDTO, UserProfileModel>();
                    cfg.CreateMap<DepartmentDTO, DepartmentModel>();
                });
                
                detailedModel.UserProfile = Mapper.Map<UserDTO, UserProfileModel>(userDto);
                
                detailedModel.Department = userDto.DepartmentId == null
                    ? null
                    : Mapper.Map<DepartmentDTO, DepartmentModel>(departmentDto);
                
                model.Add(detailedModel);
            }

            return View(model);
        }

        public ActionResult Details(int? id)
        {
            var departmentDto = new DepartmentDTO();

            var userDto = userService.GetUser(id);

            if (userDto.DepartmentId != null)
            {
                departmentDto = departmentService.GetDepartment(userDto.DepartmentId);
            }

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<UserDTO, UserProfileModel>();
                cfg.CreateMap<DepartmentDTO, DepartmentModel>();
            });

            var detailedInfo = new DetailedUserProfileModel()
            {
                UserProfile = Mapper.Map<UserDTO, UserProfileModel>(userDto),
                Department = Mapper.Map<DepartmentDTO, DepartmentModel>(departmentDto)
            };

            return View(detailedInfo);
        }
    }
}