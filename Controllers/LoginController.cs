using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Seminar_1.Models;
using System.Security.Claims;
using System.Text;

namespace Seminar_1.Controllers
{
    public class LoginController : Controller
    {
        private readonly IWebHostEnvironment hostEnvironment;
        private readonly Seminar1Context context;

        public LoginController(IWebHostEnvironment hostEnvironment, Seminar1Context context)
        {
            this.hostEnvironment = hostEnvironment;
            this.context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new LoginVM());
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "There were some errors in your form");
                return View(loginVM);
            }

            var users = context.Users.ToList();
            var user = users.FirstOrDefault(u => u.UserName == loginVM.UserName);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid Credentials");
                return View(loginVM);
            }

            if (user.Password != hashPass(loginVM.Password))
            {
                ModelState.AddModelError(string.Empty, "Invalid Credentials");
                return View(loginVM);
            }

            List<Claim> claims = new List<Claim>()
            {
                new Claim("UserName", loginVM.UserName),
            };

            ClaimsIdentity claimsIdentity= new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = false,
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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
