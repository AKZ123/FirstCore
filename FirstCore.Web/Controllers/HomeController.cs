using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FirstCore.Web.Models;
using FirstCore.Web.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FirstCore.Web.Controllers
{
    public class HomeController : Controller
    {
        //Part:18,53,64
        private readonly IEmployeeRepository _employeeRepository;
        //private readonly IHostingEnvironment hostingEnvironment;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly ILogger<HomeController> logger;

        public HomeController(IEmployeeRepository employeeRepository, IWebHostEnvironment hostingEnvironment, ILogger<HomeController> logger)
        {
            _employeeRepository = employeeRepository;
            this.hostingEnvironment = hostingEnvironment;
            this.logger = logger;
        }

        public IActionResult Index()
        {
            //Part:27
            var model = _employeeRepository.GetAllEmployee();
            return View(model);
        }

        //Part:20, 33
        //[Route("Home/Details/{id?}")]
        public ViewResult Details(int? id)
        {
            //Part:64
            //logger.LogTrace("Trace Log");
            //logger.LogDebug("Debug Log");
            //logger.LogInformation("Information Log");
            //logger.LogWarning("Warning Log");
            //logger.LogError("Error Log");
            //logger.LogCritical("Critical Log");

            //throw new Exception("An Error in Detail View");        //60  

            //Employee model = _employeeRepository.GetEmployee(1);
            //ViewBag.PageTitle="Employee Detsil";
            //return View(model);

            //Part: 57
            Employee employee = _employeeRepository.GetEmployee(id.Value);

            if (employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound",id.Value);
            }

            //Part:26
            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Employee = employee,   //_employeeRepository.GetEmployee(id ?? 1),
                PageTitle = "This is Employee Details page"
            };

            return View(homeDetailsViewModel);
        }

        //Part:40
        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }

        //Part:41,42,  (53,54)
        [HttpPost]
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if (model.Photos != null && model.Photos.Count > 0)
                {
                    foreach (IFormFile photo in model.Photos)
                    {
                        string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                        uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        photo.CopyTo(new FileStream(filePath, FileMode.Create));
                    }
                }

                Employee newEmployee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = uniqueFileName
                };

                _employeeRepository.Add(newEmployee);
                return RedirectToAction("Details", new { id = newEmployee.Id });
            }
            return View();
        }
        //public JsonResult Details()
        //{
        //    Employee model = _employeeRepository.GetEmployee(1);
        //    return Json(model);
        //}


        //public IActionResult Index()
        //{
        //    return View();
        //}

        //Part:55
        [HttpGet]
        public ViewResult Edit(int id)
        {
            Employee employee = _employeeRepository.GetEmployee(id);
            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email=employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath
            };
            return View(employeeEditViewModel);
        }

        //Part: 56
        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = _employeeRepository.GetEmployee(model.Id);
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;

                if (model.Photos != null)
                {
                    if (model.ExistingPhotoPath != null)
                    {
                        string filePath=Path.Combine(hostingEnvironment.WebRootPath, "images", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    employee.PhotoPath = processUploadedFile(model);
                }
     
                Employee updateEmployee = _employeeRepository.Update(employee);
                return RedirectToAction("index");
            }
            return View();
        }

        private string processUploadedFile(EmployeeEditViewModel model)
        {
            string uniqueFileName = null;
            if (model.Photos != null && model.Photos.Count > 0)
            {
                foreach (IFormFile photo in model.Photos)
                {
                    string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using(var fileStream= new FileStream(filePath, FileMode.Create))
                    {
                        photo.CopyTo(fileStream);
                    }
                }
            }
            return uniqueFileName;
        }
    }
}