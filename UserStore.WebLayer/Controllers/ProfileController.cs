using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using UserStore.BusinessLayer.DTO;
using UserStore.BusinessLayer.Infrastructure;
using UserStore.BusinessLayer.Interfaces;
using UserStore.WebLayer.Models;
using PagedList;

namespace UserStore.WebLayer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProfileController : Controller
    {
        private IUserService userService;
        private IDepartmentService departmentService;
        private IAuthService authService;

        public ProfileController(IUserService userServ, IDepartmentService depServ, IAuthService authServ)
        {
            userService = userServ;
            departmentService = depServ;
            authService = authServ;
        }

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
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

            IOrderedEnumerable<DetailedUserProfileModel> sortModel;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdSortParam = sortOrder == "Id_Asc" ? "Id_Desc" : "Id_Asc";
            ViewBag.DepartmentSortParam = sortOrder == "Department_Asc" ? "Department_Desc" : "Department_Asc";
            ViewBag.NameSortParam = String.IsNullOrEmpty(sortOrder) ? "Name_Desc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                model = model.Where(s => s.UserProfile.Name.Contains(searchString)).ToList();
            }

            switch (sortOrder)
            {
                case "Id_Desc":
                    sortModel = model.OrderByDescending(s => s.UserProfile.Id);
                    break;
                case "Id_Asc":
                    sortModel = model.OrderBy(s => s.UserProfile.Id);
                    break;
                case "Department_Desc":
                    sortModel = model.OrderByDescending(s => s.Department.Name);
                    break;
                case "Department_Asc":
                    sortModel = model.OrderBy(s => s.Department.Name);
                    break;
                case "Name_Desc":
                    sortModel = model.OrderByDescending(s => s.UserProfile.Name);
                    break;
                default:
                    sortModel = model.OrderBy(s => s.UserProfile.Name);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(sortModel.ToPagedList(pageNumber, pageSize));
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

            var departments = GetDepartmentsList();

            ViewBag.Departments = departments;

            return View(model);
        }

        public ActionResult Delete(int? id)
        {
            var userDto = userService.GetUser(id);

            Mapper.Initialize(cfg => cfg.CreateMap<UserDTO, UserProfileModel>());

            var model = Mapper.Map<UserDTO, UserProfileModel>(userDto);

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
            var userDto = Mapper.Map<UserProfileModel, UserDTO>(model.UserProfile);

            OperationDetails operationDetails = await userService.Update(userDto);

            if (operationDetails.Succeeded)
                return RedirectToAction("Index");

            ModelState.AddModelError(operationDetails.Property, operationDetails.Message);

            return View(model);
        }

        public ActionResult Create()
        {
            var departments = GetDepartmentsList();
            ViewBag.Departments = departments;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserProfileModel model)
        {
            var departments = GetDepartmentsList();
            ViewBag.Departments = departments;

            if (!ModelState.IsValid)
                return View(model);

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<UserProfileModel, AppUserDTO>()
                    .ForMember("UserName", opt => opt.MapFrom("Email"))
                    .ForMember("Password", opt => opt.UseValue("123456"));
                cfg.CreateMap<UserProfileModel, UserDTO>();
            });

            var appUser = Mapper.Map<UserProfileModel, AppUserDTO>(model);
            var userDto = Mapper.Map<UserProfileModel, UserDTO>(model);

            OperationDetails operationDetails = await authService.Create(appUser);

            if (operationDetails.Succeeded)
            {
                operationDetails = await userService.CreateProfile(userDto);

                if (operationDetails.Succeeded)
                    return RedirectToAction("Index");

                ModelState.AddModelError(operationDetails.Property, operationDetails.Message);
            }
            else
            {
                ModelState.AddModelError(operationDetails.Property, operationDetails.Message);
            }

            return View(model);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<ActionResult> ConfirmedDelete(UserProfileModel model)
        {
            OperationDetails operationDetails = await userService.Delete(model.Id);

            if (operationDetails.Succeeded)
            {
                operationDetails = await authService.Delete(model.Id);

                if (operationDetails.Succeeded)
                    return RedirectToAction("Index");

                ModelState.AddModelError(operationDetails.Property, operationDetails.Message);
            }
            else
            {
                ModelState.AddModelError(operationDetails.Property, operationDetails.Message);
            }

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