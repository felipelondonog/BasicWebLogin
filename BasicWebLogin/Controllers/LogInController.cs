using BasicWebLogin.Models;
using BasicWebLogin.Services;
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

                if (!user.EmailConfirmed)
                {
                    ViewBag.Message = "You have to confirm your email to continue.";
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

                await SendConfirmationEmail(user);

                _context.UserModels.Add(user);
                await _context.SaveChangesAsync();
                TempData["ConfirmMessage"] = "Your account is disabled until you confirm your account, please check your email.";
                TempData.Keep();
                return RedirectToAction("LogIn", "LogIn");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(loginModel);
            }
        }

        public ActionResult ConfirmEmail(string token)
        {
            try
            {
                // Changes EmailConfirmed state on DB
                int n = _context.UserModels
                    .Where(u => u.Token == token)
                    .ExecuteUpdate(u => u.SetProperty(p => p.EmailConfirmed, p => true));

                if (n == 0) ViewBag.Response = false;
                else ViewBag.Response = true;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = "We're sorry, we couldn't confirm your email, please try again. " + ex.Message;
                return RedirectToAction("LogIn", "LogIn");
            }
        }

        private async Task<IActionResult> SendConfirmationEmail(UserModel user)
        {
            try
            {
                // Sends confirmation E-mail
                // Define the path where the template is located and gets the file content
                string path = "Templates/ConfirmEmail.html";
                string content = System.IO.File.ReadAllText(path);
                // Builds the url and replaces it into the content
                string url = String.Format("{0}://{1}{2}", Request.Scheme, Request.Headers["Host"], "/LogIn/ConfirmEmail?token=" + user.Token);
                string htmlBody = String.Format(content, user.UserName, url);
                // Sends the email
                await EmailService.SendEmail(new EmailModel()
                {
                    To = user.Email,
                    Subject = "Confirm your account",
                    Content = htmlBody
                });

                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
