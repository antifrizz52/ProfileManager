using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using UserStore.BusinessLayer.DTO;
using UserStore.BusinessLayer.Infrastructure;
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

        public ActionResult Edit(int? id)
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

            var model = new DetailedUserProfileModel()
            {
                UserProfile = Mapper.Map<UserDTO, UserProfileModel>(userDto),
                Department = Mapper.Map<DepartmentDTO, DepartmentModel>(departmentDto)
            };

            var items = GetDepartmentsList();

            ViewBag.Departments = items;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(DetailedUserProfileModel model)
        {
            var items = GetDepartmentsList();
            ViewBag.Departments = items;

            if (!ModelState.IsValid) return View(model);

            Mapper.Initialize(cfg => cfg.CreateMap<UserProfileModel, UserDTO>());
            var userDto= Mapper.Map<UserProfileModel, UserDTO>(model.UserProfile);

            OperationDetails operationDetails = await userService.Update(userDto);

            if(operationDetails.Succeeded)
                return RedirectToAction("Index");

            ModelState.AddModelError(operationDetails.Property, operationDetails.Message);

            return View(model);
        }

        public List<SelectListItem> GetDepartmentsList()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            var departmentsDto = departmentService.GetDepartments();

            Mapper.Initialize(cfg => cfg.CreateMap<DepartmentDTO, DepartmentModel>());
            var departments = Mapper.Map<IEnumerable<DepartmentDTO>, List<DepartmentModel>>(departmentsDto);

            foreach (var dep in departments)
            {
                items.Add(new SelectListItem { Text = dep.Name, Value = dep.Id.ToString() });
            }

            return items;
        } 
    }
}