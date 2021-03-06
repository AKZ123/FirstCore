﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FirstCore.Web.Models;
using FirstCore.Web.Security;
using FirstCore.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FirstCore.Web.Controllers
{
    [Authorize]   //71.3
    public class HomeController : Controller
    {
        //Part:18,53,64,  120.3.1
        private readonly IEmployeeRepository _employeeRepository;
        //private readonly IHostingEnvironment hostingEnvironment;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly ILogger<HomeController> logger;

        private readonly IDataProtector protector;
        //Part:-                                                                                                                              120.3.2,                                        120.3.3                  
        public HomeController(IEmployeeRepository employeeRepository, IWebHostEnvironment hostingEnvironment, ILogger<HomeController> logger, IDataProtectionProvider dataProtectionProvider, DataProtectionPurposeStrings dataProtectionPurposeStrings)
        {
            _employeeRepository = employeeRepository;
            this.hostingEnvironment = hostingEnvironment;
            this.logger = logger;
            //120.3.4
            protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.EmployeeIdRouteValue);
        }

        [AllowAnonymous]  //71.3
        public IActionResult Index()
        {
            //Part:27
            //var model = _employeeRepository.GetAllEmployee();
            //Part: 120.5
            var model = _employeeRepository.GetAllEmployee()
                                .Select(e =>
                                {
                                    e.EncryptedId = protector.Protect(e.Id.ToString());
                                    return e;
                                });
            //
            return View(model);
        }

        //Part:20, 33
        //[Route("Home/Details/{id?}")]
        [AllowAnonymous]  //71.3
        public ViewResult Details(string id)  //(int? id)   p120
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

            //Part: 120.7.1
            int employeeId = Convert.ToInt32(protector.Unprotect(id));
            //Part: 57
            Employee employee = _employeeRepository.GetEmployee(employeeId); //(id.Value);  p120

            if (employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", employeeId);  //id.Value);  p120
            }

            //Part:26
            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Employee = employee,   //_employeeRepository.GetEmployee(id ?? 1),
                PageTitle = "This is Employee Details page"
            };

            return View(homeDetailsViewModel);
        }

        //Part:40, 
        [HttpGet]
        //[Authorize]   //71.2
        public ViewResult Create()
        {
            return View();
        }

        //Part:41,42,  (53,54)
        [HttpPost]
        //[Authorize]  //71
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

        //Part:55,
        [HttpGet]
        //[Authorize]  //71
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
        //[Authorize]  //71
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
                    employee.PhotoPath = ProcessUploadedFile(model);
                }
     
                Employee updateEmployee = _employeeRepository.Update(employee);
                return RedirectToAction("index");
            }
            return View();
        }

        private string ProcessUploadedFile(EmployeeEditViewModel model)
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