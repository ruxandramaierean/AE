using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seminar_1.Models;
using System.Text;



namespace Seminar_1.Controllers
{
    [Route("[Controller]")]
    public class RegisterController : Controller
    {
        private readonly IWebHostEnvironment hostEnvironment;
        private readonly Seminar1Context context;
        public RegisterController(IWebHostEnvironment hostEnvironment, Seminar1Context context)
        {
            this.hostEnvironment = hostEnvironment;
            this.context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new UserVM());
        }

        [HttpPost]
        public async Task<IActionResult> Index(UserVM userVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "There were some errors in your form");
                return View(userVM);
            }

            userVM.Password = hashPass(userVM.Password);
            context.Add(UserVM.VMUserToUser(userVM));
            context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        string hashPass(string password)
        {
            string hashed = Convert.ToBase64String(Encoding.ASCII.GetBytes(password));
            //Console.WriteLine($"HAshed password: {hashed}");
            return hashed;
        }
    }
}
