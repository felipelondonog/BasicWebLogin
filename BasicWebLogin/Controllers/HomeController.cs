using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using BasicWebLogin.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Claims;

namespace BasicWebLogin.Controllers
{
    public class HomeController : Controller
    {
        private readonly LoginDBContext _context; 
        private readonly ILogger<HomeController> _logger;

        public HomeController(LoginDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.User.Claims.FirstOrDefault() == null)
            {
                return RedirectToAction("LogIn", "LogIn");
            }

            string? email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;

            // Checks if user has email confirmed
            UserModel? user = await _context.UserModels.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || !user.EmailConfirmed) return RedirectToAction("LogIn", "LogIn");

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
