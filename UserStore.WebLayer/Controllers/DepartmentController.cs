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

        public ActionResult Index(string sortOrder)
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

            IOrderedEnumerable<DepartmentModel> orderedDepartments;

            ViewBag.IdSortParam = sortOrder == "Id_Asc" ? "Id_Desc" : "Id_Asc";
            ViewBag.StuffCountSortParam = sortOrder == "StuffCount_Asc" ? "StuffCount_Desc" : "StuffCount_Asc";
            ViewBag.NameSortParam = String.IsNullOrEmpty(sortOrder) ? "Name_Desc" : "";

            switch (sortOrder)
            {
                case "Id_Desc":
                    orderedDepartments = departments.OrderByDescending(s => s.Id);
                    break;
                case "Id_Asc":
                    orderedDepartments = departments.OrderBy(s => s.Id);
                    break;
                case "StuffCount_Desc":
                    orderedDepartments = departments.OrderByDescending(s => s.Users.Count);
                    break;
                case "StuffCount_Asc":
                    orderedDepartments = departments.OrderBy(s => s.Users.Count);
                    break;
                case "Name_Desc":
                    orderedDepartments = departments.OrderByDescending(s => s.Name);
                    break;
                default:
                    orderedDepartments = departments.OrderBy(s => s.Name);
                    break;
            }
            
            return View(orderedDepartments);
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Details(int? id, string sortOrder)
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

            IOrderedEnumerable<UserProfileModel> orderedUsers;

            ViewBag.IdSortParam = sortOrder == "Id_Asc" ? "Id_Desc" : "Id_Asc";
            ViewBag.AddressSortParam = sortOrder == "Address_Asc" ? "Address_Desc" : "Address_Asc";
            ViewBag.NameSortParam = String.IsNullOrEmpty(sortOrder) ? "Name_Desc" : "";

            switch (sortOrder)
            {
                case "Id_Desc":
                    orderedUsers = model.Users.OrderByDescending(s => s.Id);
                    break;
                case "Id_Asc":
                    orderedUsers = model.Users.OrderBy(s => s.Id);
                    break;
                case "Address_Desc":
                    orderedUsers = model.Users.OrderByDescending(s => s.Address);
                    break;
                case "Address_Asc":
                    orderedUsers = model.Users.OrderBy(s => s.Address);
                    break;
                case "Name_Desc":
                    orderedUsers = model.Users.OrderByDescending(s => s.Name);
                    break;
                default:
                    orderedUsers = model.Users.OrderBy(s => s.Name);
                    break;
            }

            model.Users = orderedUsers.ToList();

            return View(model);
        }

        public ActionResult Edit(int? id)
        {
            var department = departmentService.GetDepartment(id);

            Mapper.Initialize(cfg => cfg.CreateMap<DepartmentDTO, DepartmentModel>());
            var model = Mapper.Map<DepartmentDTO, DepartmentModel>(department);

            return View(model);
        }

        public ActionResult Delete(int? id)
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

        [HttpPost]
        [ActionName("Delete")]
        public async Task<ActionResult> ConfirmedDelete(DepartmentModel model)
        {
            OperationDetails operationDetails = await departmentService.Delete(model.Id);

            if(operationDetails.Succeeded)
                return RedirectToAction("Index");

            ModelState.AddModelError(operationDetails.Property, operationDetails.Message);

            return View(model);
        }
    }
}