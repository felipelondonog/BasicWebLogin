using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace BasicWebLogin.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public IActionResult Index()
        {
            if (HttpContext.User.Claims.FirstOrDefault() == null)
            {
                return RedirectToAction("LogIn", "LogIn");
            }
            ViewData["SessionUser"] = HttpContext.User.Claims.FirstOrDefault().Value;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("LogIn", "LogIn");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
