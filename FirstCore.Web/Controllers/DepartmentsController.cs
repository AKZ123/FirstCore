using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FirstCore.Web.Controllers
{
    public class DepartmentsController : Controller
    {
        public string List()
        {
            return "List view() of Department";
        }

        public string Details()
        {
            return "Detail View() of Deartment";
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}