using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Seminar_1.Controllers
{
    public class HomeController : Controller
    { 

        public HomeController(ILogger<HomeController> logger)
        {
           
        }

        public IActionResult Index()
        {
            return View();
        }       
    }
}