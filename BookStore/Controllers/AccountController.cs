using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BookStore.Data;
using BookStore.Extensions;
using BookStore.Models;
using static BookStore.Utility.DeleteFile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace BookStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly IHostingEnvironment _env;
        private readonly BookContext _db;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 IEmailSender emailSender,
                                 IHostingEnvironment env,
                                 BookContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _env = env;
            _db = db;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginModel, string ReturnUrl = "/")
        {
            if (!ModelState.IsValid) return View(loginModel);

            var result = await _signInManager.PasswordSignInAsync(loginModel.Email, loginModel.Password, loginModel.RemmemberMe, true);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Email or passwords are wrong!");
                return View(loginModel);
            }

            return Redirect(ReturnUrl);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            if (!ModelState.IsValid) return View(registerModel);

            var user = new ApplicationUser
            {
                Email = registerModel.Email,
                NormalizedEmail = registerModel.Email.ToUpper(),
                UserName = registerModel.Email,
                NormalizedUserName = registerModel.Email.ToUpper(),
            };

            var result = await _userManager.CreateAsync(user, registerModel.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(registerModel);
            }

            await _userManager.AddToRoleAsync(user, Roles.Member);

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var callbackUrl = Url.Action(nameof(ConfirmEmail), "Account", new { userId = user.Id, code }, Request.Scheme);

            await _emailSender.SendEmailAsync(registerModel.Email, "Email Confirmation",
                $"You can confirm email ==> <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>here</a>.");

            return RedirectToAction(nameof(ConfirmInfo));
        }

        public IActionResult ConfirmInfo()
        {
            return View();
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return NotFound();

            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View();
            }

            await _signInManager.SignInAsync(user, true);

            return View();
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Details()
        {
            var user = await _userManager.GetUserAsync(User);

            return View(user);
        }

        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ApplicationUser user)
        {
            var userFromDb = await _userManager.GetUserAsync(User);

            if (user.Photo != null)
            {
                if (!user.Photo.IsPhoto())
                {
                    ModelState.AddModelError("Photo", "File must be image.");
                    return View(user);
                }

                if (!string.IsNullOrEmpty(userFromDb.Image))
                {
                    Delete(Path.Combine(_env.WebRootPath, userFromDb.Image));
                }

                userFromDb.Image = await user.Photo.SavePhotoAsync(_env.WebRootPath, "profile");
            }

            userFromDb.Firstname = user.Firstname;
            userFromDb.Lastname = user.Lastname;
            userFromDb.Birthdate = user.Birthdate;

            _db.Entry(userFromDb).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return View(userFromDb);
        }
    }
}