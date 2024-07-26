using BasicWebLogin.Models;
using BasicWebLogin.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BasicWebLogin.Controllers
{
    public class LogInController : Controller
    {
        private readonly LoginDBContext _context;

        public LogInController(LoginDBContext context)
        {
            _context = context;
        }

        public IActionResult LogIn()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(LogIn loginModel)
        {
            try
            {
                UserModel? user = await _context.UserModels.FirstOrDefaultAsync(u => u.Email == loginModel.Email);

                if (user == null)
                {
                    ViewBag.Message = "User not found.";
                    return View(loginModel);
                }

                if(user.Pwd != GeneralUtilities.ConvertStringtoSHA256(loginModel.Pwd))
                {
                    ViewBag.Message = "Incorrect password.";
                    return View(loginModel);
                }

                // Creates scheme for keep user logged in using cookies
                List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, loginModel.Email)
                };

                ClaimsIdentity claimsId = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                AuthenticationProperties authProps = new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    IsPersistent = loginModel.KeepLoggedIn
                };

                TempData["SessionUser"] = loginModel.Email;
                TempData.Keep();

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsId), authProps);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(LogIn loginModel)
        {
            try
            {
                //if (ModelState.IsValid)
                //{
                    // Checks the password are the same before saving changes
                    if (loginModel.Pwd != loginModel.ConfirmPassword)
                    {
                        ViewBag.Message = "The passwords do not match.";
                        return View(loginModel);
                    }

                    // Checks the user email is not already used.
                    if (await _context.UserModels.FirstOrDefaultAsync(u => u.Email == loginModel.Email) != null)
                    {
                        ViewBag.Message = "The email is already registered.";
                        return View(loginModel);
                    }

                    UserModel user = loginModel;
                    // Creates a new Token for the user
                    user.Token = GeneralUtilities.CreateToken();

                    // Encodes the password before saving
                    user.Pwd = GeneralUtilities.ConvertStringtoSHA256(user.Pwd);
                    user.ResetPassword = false;
                    user.EmailConfirmed = false;

                    _context.UserModels.Add(user);
                    await _context.SaveChangesAsync();
                ViewBag.Message = "Your account is disabled until you confirm your account, please check your email.";
                    return RedirectToAction("LogIn", "LogIn");
                //}

                //return View(loginModel);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
