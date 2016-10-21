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
    public class DepartmentController : Controller
    {
        private IDepartmentService departmentService;

        public DepartmentController(IDepartmentService service)
        {
            departmentService = service;
        }

        public ActionResult Index()
        {
            var depDTO = departmentService.GetDepartments();

            Mapper.Initialize(cfg => cfg.CreateMap<DepartmentDTO, DepartmentModel>());
            var departments = Mapper.Map<IEnumerable<DepartmentDTO>, List<DepartmentModel>>(depDTO);

            foreach (var department in departments)
            {
                var usersDto = departmentService.GetAssociatedUsers(department.Id);

                Mapper.Initialize(cfg => cfg.CreateMap<UserDTO, UserProfileModel>());
                department.Users = Mapper.Map<IEnumerable<UserDTO>, List<UserProfileModel>>(usersDto);
            }

            return View(departments);
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Details(int? id)
        {
            var department = departmentService.GetDepartment(id);
            var users = departmentService.GetAssociatedUsers(id);

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<DepartmentDTO, DepartmentModel>();
                cfg.CreateMap<UserDTO, UserProfileModel>();
            });

            var model = Mapper.Map<DepartmentDTO, DepartmentModel>(department);
            model.Users = Mapper.Map<IEnumerable<UserDTO>, List<UserProfileModel>>(users);

            return View(model);
        }

        public ActionResult Edit(int? id)
        {
            var department = departmentService.GetDepartment(id);

            Mapper.Initialize(cfg => cfg.CreateMap<DepartmentDTO, DepartmentModel>());
            var model = Mapper.Map<DepartmentDTO, DepartmentModel>(department);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(DepartmentModel model)
        {
            if (!ModelState.IsValid) return View();

            Mapper.Initialize(cfg => cfg.CreateMap<DepartmentModel, DepartmentDTO>());
            var department = Mapper.Map<DepartmentModel, DepartmentDTO>(model);

            OperationDetails operationDetails = await departmentService.Update(department);

            if (operationDetails.Succeeded)
                return RedirectToAction("Index");

            ModelState.AddModelError(operationDetails.Property, operationDetails.Message);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(DepartmentModel model)
        {
            if (!ModelState.IsValid) return View(model);

            Mapper.Initialize(cfg => cfg.CreateMap<DepartmentModel, DepartmentDTO>());
            var department = Mapper.Map<DepartmentModel, DepartmentDTO>(model);

            OperationDetails operationDetails = await departmentService.Create(department);

            if (operationDetails.Succeeded)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(operationDetails.Property, operationDetails.Message);

            return View(model);
        }
    }
}