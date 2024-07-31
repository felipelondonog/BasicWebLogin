using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BasicWebLogin.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Security.Principal;
using BasicWebLogin.Utilities;

namespace BasicWebLogin.Controllers
{
    public class UserModelsController : Controller
    {
        private readonly LoginDBContext _context;

        public UserModelsController(LoginDBContext context)
        {
            _context = context;
        }

        // GET: UserModels/MyAccount
        public async Task<IActionResult> MyAccount()
        {
            UserModel user = await GetUserModel();

            if (user == null) return NotFound();

            return View(user);
        }

        // GET: UserModels/Edit/5
        public async Task<IActionResult> Edit()
        {
            UserModel user = await GetUserModel();

            if (user == null) return NotFound();

            return View(user);
        }

        // POST: UserModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserModel userModel)
        {
            UserModel user = await GetUserModel();

            if (user == null) return NotFound();

            if(userModel.Id != user.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userModel);
                    await _context.SaveChangesAsync();

                    // TODO: Update claims for showing new username when updated.
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserModelExists(userModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("MyAccount", "UserModels");
            }
            return View(userModel);
        }

        // GET: UserModels/Delete/5
        public async Task<IActionResult> Delete()
        {
            UserModel? user = await GetUserModel();
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: UserModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userModel = await _context.UserModels.FindAsync(id);
            if (userModel != null)
            {
                _context.UserModels.Remove(userModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("LogOut", "Home");
        }

        public async Task<IActionResult> ChangePassword()
        {
            UserModel? user = await GetUserModel();
            if (user == null)
            {
                return NotFound();
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string password, string newPassword, string confirmPassword)
        {
            UserModel currentUser = await GetUserModel();

            // Checks the old password is correct
            if (GeneralUtilities.ConvertStringtoSHA256(password) != currentUser.Pwd)
            {
                ViewBag.Message = "The password is incorrect.";
                return View();
            }

            // Checks new password and its confirmation are correct
            if (newPassword != confirmPassword)
            {
                ViewBag.Message = "The passwords do not match.";
                return View();
            }

            currentUser.Pwd = GeneralUtilities.ConvertStringtoSHA256(newPassword);

            _context.UserModels.Update(currentUser);
            await _context.SaveChangesAsync();

            ViewBag.Message = "Your password has been updated.";

            return RedirectToAction("MyAccount", "UserModels");
        }

        private bool UserModelExists(int id)
        {
            return _context.UserModels.Any(e => e.Id == id);
        }

        private string? GetUserEmail() => HttpContext.User.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        private async Task<UserModel> GetUserModel() => await _context.UserModels.AsNoTracking().FirstOrDefaultAsync(u => u.Email == GetUserEmail());
    }
}
