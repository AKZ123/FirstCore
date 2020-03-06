using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirstCore.Web.Models;
using FirstCore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FirstCore.Web.Controllers
{
    public class HomeController : Controller
    {
        //Part:18
        private readonly IEmployeeRepository _employeeRepository;

        public HomeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
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
            //Employee model = _employeeRepository.GetEmployee(1);
            //ViewBag.PageTitle="Employee Detsil";
            //return View(model);

            //Part:26
            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Employee = _employeeRepository.GetEmployee(id ?? 1),
                PageTitle = "This is Details page"
            };

            return View(homeDetailsViewModel);
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
    }
}